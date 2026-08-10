using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.CheckInDtos;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class CheckInService(ICheckInRepo checkInRepo, IReservationRepo reservationRepo,IUserRepo userRepo, IRoomRepo roomRepo, IMapper mapper) : ICheckInService
    {
        private readonly ICheckInRepo _checkInRepo = checkInRepo;
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IReservationRepo _reservationRepo = reservationRepo;
        private readonly IRoomRepo _roomRepo = roomRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<bool>> CompleteCheckOutAsync(Guid checkInId, CancellationToken cancellationToken = default)
        {
            var checkIn = await _checkInRepo.GetCheckInByIdAsync(checkInId, cancellationToken);
            if (checkIn is null) return Response<bool>.Failure(["Checkin Not Found "]);
            if (!checkIn.IsCheckInActive) return Response<bool>.Failure(["CheckIn not active, guest has already checked out"]);
            checkIn.CompleteCheckut(); //   Business logic execution
            await _checkInRepo.UpdateCheckInAsync(checkIn, cancellationToken);
            return Response<bool>.Success(true);
        }

        public async Task<Response<CreateCheckInResponseDto>> CreateCheckInAsync(CreateCheckInRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var reservation = await _reservationRepo.GetReservationByIdAsync(requestDto.ReservationId, cancellationToken);
            if (reservation is null)
            {
                return Response<CreateCheckInResponseDto>.Failure(["Reservation Not Found"]);
            }
            if (reservation.ReservationStatus != ReservationStatus.Confirmed)
            {
                return Response<CreateCheckInResponseDto>.Failure([$"Cannot Check In, reservation status is currently {reservation.ReservationStatus} "]);
            }
            var room = await _roomRepo.GetByIdAsync(requestDto.RoomId, cancellationToken);

            if (room is null || !room.IsAvailable) return Response<CreateCheckInResponseDto>.Failure(["Room is not available for checkin"]);
            var checkIn = new CheckIn()
            {
                RoomId = requestDto.RoomId,
                UserId = reservation.UserId,
                ReservationId = reservation.Id,
            };
            room.CheckIn();
            reservation.ReservationStatus = ReservationStatus.CheckedIn;
            // save changes
            var checkInResponse = await _checkInRepo.CreateCheckInAsync(checkIn, cancellationToken);
            await _reservationRepo.UpdateReservationAsync(reservation, cancellationToken);
            await _roomRepo.UpdateRoomAsync(room, cancellationToken);

            var result = _mapper.Map<CreateCheckInResponseDto>(checkInResponse);
            return Response<CreateCheckInResponseDto>.Success(result);
        }

        public async Task<Response<CheckInResponseDto?>> GetActiveCheckInByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var room = await _roomRepo.GetByRoomNumberAsync(roomNumber.ToUpperInvariant(), cancellationToken);

            if (room is null) return Response<CheckInResponseDto?>.Failure([$"Room with room number {roomNumber} not found"]);
            
            var checkIn = await _checkInRepo.GetActiveCheckInByRoomNumberAsync(roomNumber.ToUpperInvariant(), cancellationToken);
            if (checkIn is null) return Response<CheckInResponseDto?>.Failure([$"No active CheckIn for Room{roomNumber}"]);
            var result = _mapper.Map<CheckInResponseDto>(checkIn);
            return Response<CheckInResponseDto?>.Success(result);
        }

        public async Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetAllActiveCheckInsAsync(CancellationToken cancellationToken= default)
        {
            var activeCheckIns = await _checkInRepo.GetAllActiveCheckInsAsync(cancellationToken);
            var result = activeCheckIns.Select(c => _mapper.Map<CheckInResponseDto>(c)).ToList();
            return Response<IReadOnlyCollection<CheckInResponseDto>>.Success(result);
        }

        public async Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetRoomCheckInHistoryAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var room = await _roomRepo.GetByRoomNumberAsync(roomNumber.ToUpperInvariant(), cancellationToken);
            if (room is null) return Response<IReadOnlyCollection<CheckInResponseDto>>.Failure(["Room Not Found"]);

            var allRoomCheckIns = await _checkInRepo.GetRoomCheckInHistoryAsync(roomNumber.ToUpperInvariant(), cancellationToken);
            var result = allRoomCheckIns.Select(c => _mapper.Map<CheckInResponseDto>(c)).ToList();
            return Response<IReadOnlyCollection<CheckInResponseDto>>.Success(result);
        }

        public async Task<Response<CheckInResponseDto?>> GetCheckInByIdAsync(Guid checkInId, Roles userRole, string username, CancellationToken cancellationToken = default)
        {
            var checkIn = await _checkInRepo.GetCheckInByIdAsync(checkInId, cancellationToken);
            if (checkIn is null) return Response<CheckInResponseDto?>.Failure(["CheckIn Not Found"]);
            if (userRole == Roles.Customer)
            {
                if (checkIn.User.Username.ToUpperInvariant() != username.ToUpperInvariant())
                {
                    return Response<CheckInResponseDto?>.Failure(["Forbidden"]);
                }
            }
            var result = _mapper.Map<CheckInResponseDto>(checkIn);
            return Response<CheckInResponseDto?>.Success(result);
        }

        public async Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetCheckInsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.FindByUsernameAsync(username, cancellationToken);
            if (user is null) return Response<IReadOnlyCollection<CheckInResponseDto>>.Failure([$"User with username {username} not found"]);
            Console.WriteLine(user.Username);
            Console.WriteLine("hgfdsrtyuioiuytre");
            var userCheckIns = await _checkInRepo.GetCheckInsByUsernameAsync(username.ToUpperInvariant(), cancellationToken);
            var result = userCheckIns.Select(u => _mapper.Map<CheckInResponseDto>(u)).ToList();
            return Response<IReadOnlyCollection<CheckInResponseDto>>.Success(result);
        }
        
        
    }
}