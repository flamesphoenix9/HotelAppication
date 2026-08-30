using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Data;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class ReservationService(IReservationRepo reservationRepo,
                                    IRoomDateAllocationRepo roomDateAllocationRepo,
                                    IUnitOfWork unitOfWork,
                                    IRoomRepo roomRepo,
                                    IMapper mapper,
                                    ILogger<ReservationService> logger) : IReservationService
    {
        private readonly IReservationRepo _reservationRepo = reservationRepo;
        private readonly IRoomDateAllocationRepo _roomDateAllocationRepo = roomDateAllocationRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRoomRepo _roomRepo = roomRepo;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ReservationService> _logger = logger;

        public async Task<Response<ReservationResponseDto>> CheckInReservationAsyc(Guid reservationId, ReservationCheckInRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var reservation = await _reservationRepo.GetReservationByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Resevation with ID {reservationId} not found"]);
            }
            //  VALIDATE BUSINESS LOGIC 
            if (reservation.ReservationGroup is null || !reservation.ReservationGroup.PaymentConfirmed)
            {
                return Response<ReservationResponseDto>.Failure([$"Cannoyt validate payment for reservation group"]);
            }
            if (reservation.ReservationStatus != Models.ReservationStatusEnum.Reserved)
            {
                return Response<ReservationResponseDto>.Failure([$"Cannot check in reservation with Status {reservation.ReservationStatus}"]);
            }
            //  Room validation
            if(reservation.AssignedRoomId is null)
            {
                return Response<ReservationResponseDto>.Failure([$"No room assigned for reservation with Id: {reservationId}"]);
            }
            var assignedRoom = await _roomRepo.GetByIdAsync(reservation.AssignedRoomId.Value, cancellationToken);
            if (assignedRoom is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Assigned room does no exist: assigned room Id {reservation.AssignedRoomId}, assigned room number:{reservation.AssignedRoomNumber}"]);
            }
            if (!assignedRoom.IsAvailable)
            {
                return Response<ReservationResponseDto>.Failure([$"Assigned Room {assignedRoom.RoomNumber} is not available; room status { assignedRoom.Status}", "please assign a new room"]);
            }
            assignedRoom.CheckIn();
            reservation.CheckIn(requestDto.CheckInNyName, requestDto.CheckInByEmailAddress, DateTime.UtcNow);
            
            //  Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }

        public async Task<Response<ReservationResponseDto>> CheckOutReservatonAsync(Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await _reservationRepo.GetReservationByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Resevation with ID {reservationId} not found"]);
            }
            if (reservation.ReservationStatus != Models.ReservationStatusEnum.CheckedIn )
            {
                return Response<ReservationResponseDto>.Failure([$"Cannot check out reservaton with status {reservation.ReservationStatus}"]);
            }
            if (reservation.AssignedRoomId is null)
            {
                return Response<ReservationResponseDto>.Failure([$"No room assigned for reservation with Id: {reservationId}"]);
            }
            var assignedRoom = await _roomRepo.GetByIdAsync(reservation.AssignedRoomId.Value, cancellationToken);
            if (assignedRoom is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Assigned room nt found: ID :{reservation.AssignedRoomId}, room number {reservation.AssignedRoomNumber}"]);
            }
            assignedRoom.CheckOut();
            reservation.CheckOut(DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }

        public async Task<Response<ReservationResponseDto>> GetReservationByIdAsync(Guid reservationId, Guid? userId, bool isStaff = false, CancellationToken cancellationToken = default)
        {
            var reservation = await _reservationRepo.GetReservationByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Resevation with ID {reservationId} not found"]);
            }
            if (!isStaff && reservation.ReservationGroup is not null && reservation.ReservationGroup.UserId == userId)
            {
                return Response<ReservationResponseDto>.Failure(["403"]);
            }
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }

        public async Task<Response<ReservationResponseDto>> UpdateAssignedRoomAsync(Guid reservationId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
             var reservation = await _reservationRepo.GetReservationByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Resevation with ID {reservationId} not found"]);
            }
            var prevRoomNumber = reservation.AssignedRoomNumber;
            await _roomDateAllocationRepo.DeleteAllocationRange(reservation.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var availableRooms = await _roomRepo.FindAvailableRoomsAsync(reservation.ReservationCategory, reservation.ExpectedCheckIn, reservation.ExpectedCheckOut, cancellationToken);
            
            var newRoom = availableRooms.FirstOrDefault();
            if (newRoom is null)
            {
                return Response<ReservationResponseDto>.Failure([$"Couldn't find any available room"]);
            }
            reservation.AssignRoomToReservation(newRoom.Id, newRoom.RoomNumber);
            _logger.LogInformation("Room {roomNumber} reassigned for reservation {reservationId}; previous room {prevRoomNumber}", newRoom.RoomNumber, reservationId, prevRoomNumber);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }
    }
}