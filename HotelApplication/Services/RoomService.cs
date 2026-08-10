using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.RoomDtos;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class RoomService(IRoomRepo roomRepo, IMapper mapper):IRoomService
    {
        private readonly IRoomRepo _roomRepo = roomRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<RoomResponseDto>> CraeteRoomAsync(CreateRoomRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            if (!Enum.TryParse(typeof(RoomCategory), requestDto.RoomCategory, ignoreCase: true, out var roomCategory))
            {
                return Response<RoomResponseDto>.Failure([$"Invaid room category {requestDto.RoomCategory}"]);
            }
            var roomExists = await _roomRepo.GetByRoomNumberAsync(requestDto.RoomNumber, cancellationToken);
            if(roomExists is not null) return Response<RoomResponseDto>.Failure([$"Romm with Room Number {requestDto.RoomNumber} already exists"]);
            var room = _mapper.Map<Room>(requestDto);
            var roomResponse = await _roomRepo.CreateRoomAsync(room, cancellationToken);
            var result = _mapper.Map<RoomResponseDto>(roomResponse);
            return Response<RoomResponseDto>.Success(result);
        }
        public async Task<Response<IReadOnlyCollection<RoomResponseDto>>> GetAllRoomsAsync(CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepo.GetAllRoomsAsync(cancellationToken);
            var result = rooms.Select(r => _mapper.Map<RoomResponseDto>(r)).ToList();
            return Response<IReadOnlyCollection<RoomResponseDto>>.Success(result);
        }

        public async Task<Response<IReadOnlyCollection<RoomResponseDto>>> GetAvailableRoomsAsync(CancellationToken cancellationToken = default)
        {
            var rooms = await _roomRepo.GetAvalaibleRoomsAsync(cancellationToken);
            var result = rooms.Select(r => _mapper.Map<RoomResponseDto>(r)).ToList();
            return Response<IReadOnlyCollection<RoomResponseDto>>.Success(result);
        }

        public async Task<Response<RoomResponseDto?>> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken= default)
        {
            var room = await _roomRepo.GetByIdAsync(roomId, cancellationToken);
            if (room is null) return Response<RoomResponseDto?>.Failure(["Room Not Found"]);
            var result = _mapper.Map<RoomResponseDto>(room);
            return Response<RoomResponseDto?>.Success(result);
        }
        
        public async Task<Response<bool>> UpdateRoomStatusAsync(UpdateRoomStatusDto updateRoomStatusDto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(updateRoomStatusDto.roomStatus) || !Enum.TryParse<RoomStatus>(updateRoomStatusDto.roomStatus, true, out var status))
            {
                return Response<bool>.Failure(["Invalid status"]);
            }
            var room = await _roomRepo.GetByIdAsync(updateRoomStatusDto.roomId, cancellationToken);

            if (room is null) return Response<bool>.Failure(["Room Not Found"]);
            try
            {
                switch (status)
                {
                    case RoomStatus.Available:
                        room.MarkAvailable();
                        break;
                    case RoomStatus.UnderMaintenance:
                        room.MarkForMaintenance();
                        break;
                    case RoomStatus.Dirty:
                        room.MarkDirty();
                        break;
                    default:
                        return Response<bool>.Failure(["Invalid or unhandled room status."]);
                }
            }
            catch (Exception e)
            {
                return Response<bool>.Failure(new List<string>() { e.Message });
            }
            await _roomRepo.UpdateRoomAsync(room, cancellationToken);
            return Response<bool>.Success(true);
        }
    }
}