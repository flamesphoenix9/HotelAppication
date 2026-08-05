using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Dtos;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Identity.Data;

namespace HotelApplication.Services
{
    public interface IUserService
    {
        Task<Response<UserResponseDto>> RegisterAsync(CreateUserRequestDto requestDto, CancellationToken
        cancellationToken);
        Task<Response<UserResponseDto>> ViewProfileAsyync(Guid userId, CancellationToken cancellationToken);
        Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto, CancellationToken cancellationToken);


    }
}