using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Services
{
    public class ExpiredReservationGroupCleanupWorker(IServiceProvider serviceProvider,
                                                IConfiguration configuration,
                                                ILogger<ExpiredReservationGroupCleanupWorker> logger)
                                                : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ExpiredReservationGroupCleanupWorker> _logger = logger;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background Service: Reservation Clean up service started");
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var reservationExpiryThresholdInMinutes = _configuration.GetValue<int>("ReservationExpirationInMinutes");
                    if (reservationExpiryThresholdInMinutes == default)
                    {
                        _logger.LogWarning("Reservation Expiry threshold not provided");
                        reservationExpiryThresholdInMinutes = 15;
                    }
                    var expiredDateTime = DateTime.UtcNow.AddMinutes(-reservationExpiryThresholdInMinutes);
                    var expiredReservationGroups = await dbContext.ReservationGroups
                        .Include(rg => rg.Reservations)
                        .Where(rg => rg.ReservationGroupStatus == Models.GroupReservationStatus.PendingPayment &&
                                rg.CreatedAt < expiredDateTime).ToListAsync(stoppingToken);
                    if (expiredReservationGroups.Count > 0)
                    {
                        var expiredIds = expiredReservationGroups.Select(e => e.Id).ToList();
                        var roomAllocationsToRemove = await dbContext.RoomDateAllocations.Where(rda => expiredIds.Contains(rda.ReservationGroupId)).ToListAsync();
                        dbContext.RemoveRange(roomAllocationsToRemove);
                        foreach (var reservationGroup in expiredReservationGroups)
                        {
                            reservationGroup.CancelReservationGroup();
                        }
                        var rowsAffected = await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Background Worker removed {RoomDateAllocation} room date allocations for {reservationGroupCount} reservation groups: Total rows affected {rowsAffected}",
                        roomAllocationsToRemove.Count,
                        expiredReservationGroups.Count,
                        rowsAffected);
                    }
                    _logger.LogInformation("Background Service: Reservation Clean up service finished with no errors");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured during reservation cleanup");
                }
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}