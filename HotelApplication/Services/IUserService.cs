using HotelApplication.Dtos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IUserService
    {
        Task<Response<UserResponseDto>> RegisterAsync(CreateUserRequestDto requestDto, CancellationToken
        cancellationToken);
        Task<Response<UserResponseDto>> ViewProfileAsyync(Guid userId, CancellationToken cancellationToken);
        Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto, CancellationToken cancellationToken);
        Task<Response<AdminViewUserResponseDto>> AdminRegisterUserAsync(AdminCreateUserRequestDto requestDto, CancellationToken cancellationToken = default);
        Task<Response<IReadOnlyCollection<AdminViewUserResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Response<bool>> AdminDeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Response<bool>> AddminActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    }
}