using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.RoomDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IRoomService
    {
        Task<Response<IReadOnlyCollection<RoomResponseDto>>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
        Task<Response<RoomResponseDto?>> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<RoomResponseDto>>> GetAvailableRoomsAsync(CancellationToken cancellationToken = default);
        Task<Response<RoomResponseDto>> CraeteRoomAsync(CreateRoomRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<Response<bool>> UpdateRoomStatusAsync(UpdateRoomStatusDto updateRoomStatusDto, CancellationToken cancellationToken = default);
    }
} 