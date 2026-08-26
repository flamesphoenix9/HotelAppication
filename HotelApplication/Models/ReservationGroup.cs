using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HotelApplication.Utilities;

namespace HotelApplication.Models
{
    public class ReservationGroup : EntityBase
    {
        public string ReservationGroupCode { get; init; } = ReservationGroupUtilities.GenerateGroupCode();
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPayable { get; private set; }
        public GroupReservationStatus ReservationGroupStatus { get; private set; } = GroupReservationStatus.PendingPayment;
        public bool PaymentConfirmed { get; private set; } = false;
        
        //  === USER ===
        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Navigation Property
        private readonly List<Reservation> _reservations = new();
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
        private ReservationGroup() { }
        public ReservationGroup(Guid userId)
        {
            if (userId ==Guid.Empty)
            {
                throw new InvalidOperationException($"A valid user ID is required to create reservation group. {nameof(userId)}");
            }
            UserId = userId;
        }
        public void AddReservation(Reservation reservation)
        {
            if (ReservationGroupStatus != GroupReservationStatus.PendingPayment)
            {
                throw new InvalidOperationException($"Cannot add reservtion to this reservation group: Status {ReservationGroupStatus}");
            }
            ArgumentNullException.ThrowIfNull(reservation);

            _reservations.Add(reservation);
            CalculateAmountPayable();
        }
        public void ConfirmPayment()
        {
            if (ReservationGroupStatus != GroupReservationStatus.PendingPayment)
            {
                throw new InvalidOperationException($"Cannot confirm payment: Reservation Group Status {ReservationGroupStatus}");
            }
            if (!_reservations.Any())
            {
                throw new InvalidOperationException("Cannot confirm payment for an empty reservation");
            }
            ReservationGroupStatus = GroupReservationStatus.PaymentConfirmed;
            PaymentConfirmed = true;
        }
        public void CheckInReservation(Guid reservationId, DateTime? checkInTime, string checkInByName, string checkInByEmailAddress)
        {
            if (string.IsNullOrWhiteSpace(checkInByName))
            {
                throw new ArgumentException("CheckIn By Name  invalid");
            }
            if(!MailAddress.TryCreate(checkInByEmailAddress, out _))
            {
                throw new ArgumentException($"Check In By email: Invalid mail Address{checkInByEmailAddress}");
            }
            if (ReservationGroupStatus == GroupReservationStatus.Cancelled)
            {
                throw new InvalidOperationException("Reservation already cancelled");
            }
            if (!PaymentConfirmed)
            {
                throw new InvalidOperationException("Reservation payment not confirmed");
            }
            var reservation = Reservations.FirstOrDefault(r => r.Id == reservationId) ?? throw new KeyNotFoundException($"Reservation with ID {reservationId} not found");
            reservation.CheckIn(checkInByName: checkInByName, checkInByEmailAddress: checkInByEmailAddress, checkInTime: checkInTime);
        }
        public void CheckOutReservation(Guid reservationId, DateTime? checkOutTime)
        {
            if (ReservationGroupStatus == GroupReservationStatus.Cancelled)
            {
                throw new InvalidOperationException("Reservation already cancelled");
            }
            var reservation = Reservations.FirstOrDefault(r => r.Id == reservationId) ?? throw new KeyNotFoundException($"Reservation with ID {reservationId} not found");
            reservation.CheckOut(checkOutTime);
        }
        public void CancelReservationGroup()
        {
            if (ReservationGroupStatus != GroupReservationStatus.PendingPayment)
            {
                throw new InvalidOperationException("Reservation Group cannot be cancelled");
            }
            foreach (var rsv in _reservations)
            {
                rsv.CancelReservation();
            }
            ReservationGroupStatus = GroupReservationStatus.Cancelled;
        }
        public void CalculateAmountPayable()
        {
            AmountPayable = _reservations.Sum(r => r.ReservationTotal);
        }

    }
}