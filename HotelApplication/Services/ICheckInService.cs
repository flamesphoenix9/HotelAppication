using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos.CheckInDtos;
using HotelApplication.Models;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface ICheckInService
    {
        Task<Response<CreateCheckInResponseDto>> CreateCheckInAsync(CreateCheckInRequestDto requestDto, CancellationToken cancellationToken=default);
        Task<Response<bool>> CompleteCheckOutAsync(Guid checkInId, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetAllActiveCheckInsAsync(CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetRoomCheckInHistoryAsync(string roomNumber, CancellationToken cancellationToken = default);
        Task<Response<CheckInResponseDto?>> GetActiveCheckInByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<CheckInResponseDto>>> GetCheckInsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<Response<CheckInResponseDto?>> GetCheckInByIdAsync(Guid checkInId, Roles userRole, string username, CancellationToken cancellationToken = default);
    }
}