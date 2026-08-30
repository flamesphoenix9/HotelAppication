using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.RoomTypePriceDtos;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class RoomTypePriceService(IRoomTypePriceRepo roomTypePriceRepo,
                                    IUnitOfWork unitOfWork,
                                    IMapper mapper)
                                    :IRoomTypePriceService
    {
        private readonly IRoomTypePriceRepo _roomTypePriceRepo = roomTypePriceRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<RoomTypePriceResponseDto>> CreateRoomTypePrice(CreateRoomTypePriceRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            if (!Enum.TryParse<RoomCategory>(requestDto.Category, ignoreCase: true, out var roomCategory))
            {
                return Response<RoomTypePriceResponseDto>.Failure([$"Category room Category Invalid"]);
            }
            var roomTypePriceDict = await _roomTypePriceRepo.GetAllActiveRoomTypePrice(cancellationToken);
            if (roomTypePriceDict.ContainsKey(roomCategory))
            {
                return Response<RoomTypePriceResponseDto>.Failure([$"Active price exists for category {roomCategory}"]);
            }
            var roomTypePrice = _mapper.Map<RoomTypePrice>(requestDto);
            _roomTypePriceRepo.Add(roomTypePrice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<RoomTypePriceResponseDto>(roomTypePrice);
            return Response<RoomTypePriceResponseDto>.Success(result);
        }

        public async Task<Response<RoomTypePriceResponseDto>> EditRoomTypePriceAsync(Guid roomTypePriceId, EditRoomTypeRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var roomTypePrice = await _roomTypePriceRepo.GetByIdAsync(roomTypePriceId, cancellationToken);
            if (roomTypePrice is null)
            {
                return Response<RoomTypePriceResponseDto>.Failure([$"Room ricing with ID {roomTypePriceId} not found"]);
            }
            roomTypePrice.BasePricePerNight = requestDto.BasePricePerNight;
            _roomTypePriceRepo.Update(roomTypePrice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = _mapper.Map<RoomTypePriceResponseDto>(roomTypePrice);
            return Response<RoomTypePriceResponseDto>.Success(result);
        }

        public async Task<Response<IReadOnlyCollection<RoomTypePriceResponseDto>>> GetAllRoomTypePrices(CancellationToken cancellationToken = default)
        {
            var roomTypePrices = await _roomTypePriceRepo.GetAllRoomTypePricesAsList(cancellationToken);
            if (roomTypePrices.Count < 0)
            {
                return Response<IReadOnlyCollection<RoomTypePriceResponseDto>>.Failure([$"No rom type price found"]);
            }
            var result = roomTypePrices.Select(rtp => _mapper.Map<RoomTypePriceResponseDto>(rtp)).ToList();
            return Response<IReadOnlyCollection<RoomTypePriceResponseDto>>.Success(result);
        }

        public async Task<Response<RoomTypePriceResponseDto>> GetRoomTypePriceByIdAsync(Guid roomTypePriceId, CancellationToken cancellationToken = default)
        {
            var roomTypePrice = await _roomTypePriceRepo.GetByIdAsync(roomTypePriceId, cancellationToken);
            if (roomTypePrice is null)
            {
                return Response<RoomTypePriceResponseDto>.Failure([$"Room Type Price with ID {roomTypePriceId} Not Found"]);
            }
            var result = _mapper.Map<RoomTypePriceResponseDto>(roomTypePrice);
            return Response<RoomTypePriceResponseDto>.Success(result);
        }
    }
}