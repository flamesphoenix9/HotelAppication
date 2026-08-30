using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.RoomTypePriceDtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IRoomTypePriceService
    {
        Task<Response<RoomTypePriceResponseDto>> CreateRoomTypePrice(CreateRoomTypePriceRequestDto createRoomTypePriceRequestDto, CancellationToken cancellationToken = default);
        Task<Response<RoomTypePriceResponseDto>> EditRoomTypePriceAsync(Guid roomTypePriceId, EditRoomTypeRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<RoomTypePriceResponseDto>>> GetAllRoomTypePrices(CancellationToken cancellationToken = default);
        Task<Response<RoomTypePriceResponseDto>> GetRoomTypePriceByIdAsync(Guid roomTypePriceId, CancellationToken cancellationToken = default);
    }
}