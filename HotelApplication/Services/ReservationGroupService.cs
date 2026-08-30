using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.PaymentDtos;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Dtos.ReservationGroupDtos;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class ReservationGroupService(IReservationGroupRepo reservationGroupRepo,
                                        IUserRepo userRepo,
                                        IRoomTypePriceRepo roomTypePriceRepo,
                                        IRoomRepo roomRepo,
                                        IRoomDateAllocationRepo roomDateAllocationRepo,
                                        IReservationRepo reservationRepo,
                                        IUnitOfWork unitOfWork,
                                        IMapper mapper,
                                        ILogger<ReservationGroupService> logger,
                                        IPaymentService paymentService) : IReservationGroupService
    {
        private readonly IReservationGroupRepo _reservationGroupRepo = reservationGroupRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IRoomTypePriceRepo _roomTypePriceRepo = roomTypePriceRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IRoomRepo _roomRepo = roomRepo;
        private readonly IRoomDateAllocationRepo _allocationRepo = roomDateAllocationRepo;
        private readonly IReservationRepo _reservationRepo = reservationRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReservationGroupService> _logger = logger;
        private readonly IPaymentService _paymentService = paymentService;

        public async Task<Response<ReservationGroupResponseDto>> CreateReservationGroupAsync(Guid userId, CreateReservationGroupRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.FindByIdAsync(userId, cancellationToken);
            if (user is null) return Response<ReservationGroupResponseDto>.Failure(["User not found"]);

            //  === VALIDATING INPUT
            var errors = ValidateReservationDatesAndCategories(requestDto.CreateReservationsList.ToList());
            if (errors.Count > 0) return Response<ReservationGroupResponseDto>.Failure(errors);

            //  == GET PRICE DICT
            var roomTypePriceDict = await _roomTypePriceRepo.GetAllActiveRoomTypePrice(cancellationToken);
            var reservationGroup = new ReservationGroup(userId);

            foreach (var reservationDto in requestDto.CreateReservationsList)
            {
                var category = Enum.Parse<RoomCategory>(reservationDto.ReservationCategory, true);
                if (!roomTypePriceDict.TryGetValue(category, out var pricePerNight))
                {
                    return Response<ReservationGroupResponseDto>.Failure([$"No Pricing configured for category {category}"]);
                }
                var reservation = new Reservation
                {
                    ExpectedCheckIn = reservationDto.ExpectedCheckIn,
                    ExpectedCheckOut = reservationDto.ExpectedCheckOut,
                    ReservationCategory = category,
                    PricePerNight = pricePerNight
                };
                reservationGroup.AddReservation(reservation);
            }
            reservationGroup.CalculateAmountPayable();
            _reservationGroupRepo.AddReservationGroup(reservationGroup);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
            return Response<ReservationGroupResponseDto>.Success(responseDto);
        }
        public async Task<Response<PaymentInitializeResponseDto>> InitializePaymentAsync(Guid reservationGroupId, string email, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var reservationGroup = await _reservationGroupRepo.GetReservationGroupByIdAsync(reservationGroupId, cancellationToken);
                if (reservationGroup == null) return Response<PaymentInitializeResponseDto>.Failure([$"Reservation group with ID {reservationGroupId} not found"]);
                // check perms
                if (reservationGroup.UserId != userId) return Response<PaymentInitializeResponseDto>.Failure(["403"]);
                if (reservationGroup.ReservationGroupStatus != GroupReservationStatus.PendingPayment) return Response<PaymentInitializeResponseDto>.Failure([$"Cannot process Reservation group with status {reservationGroup.ReservationGroupStatus}"]);
                if (reservationGroup.Reservations.Count < 1) return Response<PaymentInitializeResponseDto>.Failure([$"No reservation in reservation group"]);

                var assignedRoomIds = new HashSet<Guid>();
                foreach (var reservation in reservationGroup.Reservations)
                {
                    var availableRooms = await _roomRepo.FindAvailableRoomsAsync(reservation.ReservationCategory, reservation.ExpectedCheckIn, reservation.ExpectedCheckOut, cancellationToken);
                    var room = availableRooms.FirstOrDefault(r => !assignedRoomIds.Contains(r.Id));

                    if (room is null)
                    {
                        await _unitOfWork.RollBackAsync(cancellationToken);
                        return Response<PaymentInitializeResponseDto>.Failure([$"Category {reservation.ReservationCategory} is no longer available for dates {reservation.ExpectedCheckIn} through {reservation.ExpectedCheckOut}"]);
                    }
                    assignedRoomIds.Add(room.Id);
                    reservation.AssignRoomToReservation(room.Id, room.RoomNumber);
                    for (var d = reservation.ExpectedCheckIn; d < reservation.ExpectedCheckOut; d = d.AddDays(1))
                    {
                        var roomDateAllcation = new RoomDateAllocation
                        {
                            RoomId = room.Id,
                            Date = d,
                            ReservationGroupId = reservationGroup.Id,
                            ReservationId = reservation.Id
                        };
                        _allocationRepo.Add(roomDateAllcation);
                    }
                }
                reservationGroup.CalculateAmountPayable();
                var paymentInitializeRequest = new PaymentInitializeRequest(
                    Email: email,
                    Amount: reservationGroup.AmountPayable,
                    Reference: reservationGroup.ReservationGroupCode,
                    CallbackUrl: "");

                var result = await _paymentService.InitializePaymentAsync(paymentInitializeRequest, cancellationToken);
                if (result is null || !result.IsSuccess || string.IsNullOrEmpty(result.AuthorizationUrl))
                {
                    return Response<PaymentInitializeResponseDto>.Failure([result?.ErrorMessage ?? "", "Payment initialization failed"]);
                }
                var paymentInitializeResponse = new PaymentInitializeResponseDto
                {
                    AuthorizationUrl = result.AuthorizationUrl,
                    GroupCode = result.Reference ?? reservationGroup.ReservationGroupCode
                };
                _reservationGroupRepo.UpdateReservationGroup(reservationGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Response<PaymentInitializeResponseDto>.Success(paymentInitializeResponse);
            }
            catch (Exception)
            { 
                throw;
            }
        }
        //  WEBHOOK 
        public async Task<Response<bool>> ConfirmPaymentAsync(string reservationGroupCode, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _reservationGroupRepo.GetReservationGroupByCodeAsync(reservationGroupCode, cancellationToken);

            if (reservationGroup is null)
            {
                _logger.LogWarning("Payment confirmation failed: Reservation Group with code{reservationGroupCode} not found", reservationGroupCode);
                return Response<bool>.Failure([$"Reservation group '{reservationGroupCode}' not found."]);
            }

            if (reservationGroup.ReservationGroupStatus == GroupReservationStatus.PaymentConfirmed || reservationGroup.PaymentConfirmed)
            {
                return Response<bool>.Success(true);
            }
            if (reservationGroup.ReservationGroupStatus != GroupReservationStatus.PendingPayment)
            {
                return Response<bool>.Failure([$"cannot confirm payment for reservation with status {reservationGroup.ReservationGroupStatus}"]);
            }
            reservationGroup.ConfirmPayment();
            _reservationGroupRepo.UpdateReservationGroup(reservationGroup);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Payment for reservation group:  {reservationGroupCode} confirmed successfully via webhook");
            return Response<bool>.Success(true);
        }

        public async Task<Response<ReservationGroupResponseDto>> VerifyAndCnfirmPaymentAsync(string reservationGroupCode, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _reservationGroupRepo.GetReservationGroupByCodeAsync(reservationGroupCode, cancellationToken);
            if (reservationGroup is null)
            {
                _logger.LogWarning("Payment Verifivation failed: Reservation with group code {reservationGroupCode} not found", reservationGroupCode);
                return Response<ReservationGroupResponseDto>.Failure([$"Reservation with group code {reservationGroupCode} not found"]);
            }
            if (!isStaff && reservationGroup.UserId != userId)
            {
                return Response<ReservationGroupResponseDto>.Failure(["403"]);
            }

            if (reservationGroup.ReservationGroupStatus == GroupReservationStatus.PaymentConfirmed || reservationGroup.PaymentConfirmed)
            {
                var response = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
                return Response<ReservationGroupResponseDto>.Success(response);
            }
            // verify with paystack 
            var verifyResult = await _paymentService.VerifyPaymentAsync(reference: reservationGroup.ReservationGroupCode);
            if (!verifyResult.IsSuccess || verifyResult.Status != "success")
            {
                return Response<ReservationGroupResponseDto>.Failure(["Paymen has not been completed or declined"]);
            }
            if (verifyResult.Amount < reservationGroup.AmountPayable)
            {
                _logger.LogError($"Payment amount mismatch for reservation group code{reservationGroupCode}: Expected {reservationGroup.AmountPayable}, paid {verifyResult.Amount}");
                return Response<ReservationGroupResponseDto>.Failure([$"Paid amount {verifyResult.Amount} is lesser than actual reservation cost {reservationGroup.AmountPayable} "]);
            }
            reservationGroup.ConfirmPayment();
            _reservationGroupRepo.UpdateReservationGroup(reservationGroup);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var responseDto = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
            return Response<ReservationGroupResponseDto>.Success(responseDto);
        }

        public async Task<Response<ReservationGroupResponseDto>> GetReservationGroupByCodeAsync(string code, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _reservationGroupRepo.GetReservationGroupByCodeAsync(code, cancellationToken);
            if (reservationGroup is null) return Response<ReservationGroupResponseDto>.Failure([$"Reservation with code {code} not found"]);

            //  check perms
            if (!isStaff && reservationGroup.UserId != userId)
            {
                return Response<ReservationGroupResponseDto>.Failure(["403"]);

            }
            var result = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
            return Response<ReservationGroupResponseDto>.Success(result);
        }

        public async Task<Response<ReservationGroupResponseDto>> GetReservationGroupByIdAsync(Guid reservationGroupId, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _reservationGroupRepo.GetReservationGroupByIdAsync(reservationGroupId, cancellationToken);
            if (reservationGroup is null) return Response<ReservationGroupResponseDto>.Failure([$"Reservation group with ID {reservationGroupId} not found"]);
            //  check perms
            if (!isStaff && reservationGroup.UserId != userId)
            {
                return Response<ReservationGroupResponseDto>.Failure(["403"]);
            }
            var result = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
            return Response<ReservationGroupResponseDto>.Success(result);
        }
        public async Task<Response<ReservationGroupResponseDto>> CancelReservationGroupAsync(Guid reservationGroupId, Guid? userId, bool isStaff, CancellationToken cancellationToken = default)
        {
            var reservationGroup = await _reservationGroupRepo.GetReservationGroupByIdAsync(reservationGroupId, cancellationToken);
            if (reservationGroup is null) return Response<ReservationGroupResponseDto>.Failure([$"Reservation group with ID {reservationGroupId} not found", "404"]);
            //  check perms
            if (!isStaff && reservationGroup.UserId != userId)
            {
                return Response<ReservationGroupResponseDto>.Failure(["403"]);
            }
            if (reservationGroup.ReservationGroupStatus != GroupReservationStatus.PendingPayment)
            {
                return Response<ReservationGroupResponseDto>.Failure([$"Cannot cancel reservation with status: {reservationGroup.ReservationGroupStatus}"]);
            }
            reservationGroup.CancelReservationGroup();
            _reservationGroupRepo.UpdateReservationGroup(reservationGroup);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ReservationGroupResponseDto>(reservationGroup);
            return Response<ReservationGroupResponseDto>.Success(result);

        }
        public async Task<Response<IReadOnlyCollection<ReservationGroupResponseDto>>> GetAllReservationGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var reservationGroups = await _reservationGroupRepo.GetReservationGroupByUserIdAsync(userId, cancellationToken);
            if (reservationGroups.Count < 1)
            {
                return Response<IReadOnlyCollection<ReservationGroupResponseDto>>.Failure(["Reservation not found"]);
            }
            var result = reservationGroups.Select(rg => _mapper.Map<ReservationGroupResponseDto>(rg)).ToList();
            return Response<IReadOnlyCollection<ReservationGroupResponseDto>>.Success(result);
        }

         private static List<string> ValidateReservationDatesAndCategories(List<CreateReservationRequestDto> reservationRequestDtos)
        {
            var errors = new List<string>();

            if (reservationRequestDtos is null || !reservationRequestDtos.Any())
            {
                errors.Add("Must contain at least one reservation");
            }
            else
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                for (int i = 0; i < reservationRequestDtos.Count; i++)
                {
                    var reservationDto = reservationRequestDtos.ElementAt(i);
                    if (reservationDto.ExpectedCheckIn < today)
                    {
                        errors.Add($"Reservation {i + 1}: CheckIn date cannot be in the past");
                    }
                    if (reservationDto.ExpectedCheckOut < reservationDto.ExpectedCheckIn)
                    {
                        errors.Add($"Reservation {i + 1}: Checkout date must be after CheckIn date");
                    }
                    if (!Enum.TryParse<RoomCategory>(reservationDto.ReservationCategory, out _))
                    {
                        errors.Add($"Reservation {i + 1}: Invalid room category");
                    }
                }
            }
            return errors;
        }
    }
}