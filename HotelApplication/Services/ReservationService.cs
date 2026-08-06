using AutoMapper;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Mappers;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class ReservationService(IReservationRepo reservationRepo, IMapper mapper) : IReservationService
    {
        private readonly IReservationRepo _reservationRepo = reservationRepo;
        private readonly IMapper _mapper = mapper;
        public async Task<Response<CreateReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto requestDto, Guid userId, string username, CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            if (requestDto.RequestedRoomCategory != nameof(RoomCategory.Executive) && requestDto.RequestedRoomCategory != nameof(RoomCategory.Standard))
            {
                errors.Add("Invalid room category.");
            }
            if (requestDto.ExpectedCheckOut < requestDto.ExpectedCheckIn) //Compare dates
            {
                errors.Add("Check-out date cannot be earlier than check-in date");
            }
            if (errors.Count > 0)
            {
                return Response<CreateReservationResponseDto>.Failure(errors: errors);
            }

            var reservation = requestDto.ToReservationMapper(userId, username);
            var result = await _reservationRepo.CreateReservationAsync(reservation, cancellationToken);
            var response = _mapper.Map<CreateReservationResponseDto>(result);
            return Response<CreateReservationResponseDto>.Success(response);
        }
        public async Task<Response<IEnumerable<ReservationResponseDto>>> UserViewAllReservationsAsync(string username, CancellationToken cancellationToken)
        {
            var reservations = await _reservationRepo.GetAllReservationsAsync(username, cancellationToken);
            var result = reservations.Select(_mapper.Map<ReservationResponseDto>).ToList();
            return Response<IEnumerable<ReservationResponseDto>>.Success(result);
        }
        public async Task<Response<ReservationResponseDto>> UserViewReservationAsync(Guid reservationId, string username, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepo.GetReservationByUserAsync(reservationId, username, cancellationToken);
            if (reservation is null) return Response<ReservationResponseDto>.Failure(["Reservation Not found"]);
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }
         public async Task<Response<ReservationResponseDto>> StaffViewReservationAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepo.GetUserReservationByStaffAsync(reservationId, cancellationToken);
            if (reservation is null) return Response<ReservationResponseDto>.Failure(["Reservation Not found"]);
            var result = _mapper.Map<ReservationResponseDto>(reservation);
            return Response<ReservationResponseDto>.Success(result);
        }        
    }
}