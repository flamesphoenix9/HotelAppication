using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public class UserService(IUserRepo userRepo, IAuthService authService, IMapper mapper) : IUserService
    {
        private readonly IUserRepo _userRepo = userRepo;
        private readonly IAuthService _authService = authService;
        private readonly IMapper _mapper = mapper;
        public async Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto, CancellationToken cancellationToken)
        {
            var isValidEmail = _authService.IsValidEmail(requestDto.EmailAddress);
            if (!isValidEmail)
            {
                return Response<LoginResponseDto>.Failure(new List<string> { $"{requestDto.EmailAddress} is not a valid email Address" });
            }
            var user = await _userRepo.FindByEmailAsync(requestDto.EmailAddress, cancellationToken);
            if (user is null)
            {
                return Response<LoginResponseDto>.Failure(new List<string>() { $"user with email {requestDto.EmailAddress} not found" });
            }
            var verifyPassword = _authService.VerifyPassword(user, requestDto.Password);
            if (!verifyPassword) return Response<LoginResponseDto>.Failure(new List<string> { "Incorrect Password" });
            var token = _authService.GenerateJsonWebToken(user);
            var data = new LoginResponseDto { AccessToken = token };
            return Response<LoginResponseDto>.Success(data);
        }

        public async Task<Response<UserResponseDto>> RegisterAsync(CreateUserRequestDto requestDto, CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            var isValidEmail = _authService.IsValidEmail(requestDto.EmailAddress);
            if (!isValidEmail)
            {
                errors.Add($"{requestDto.EmailAddress} is not a valid email Address");
            }
            var userExists = await _userRepo.FindByEmailAsync(requestDto.EmailAddress, cancellationToken);
            if (userExists != null)
            {
                errors.Add($"User with email address {requestDto.EmailAddress} already exists");
            }
            var passwordCheck = _authService.PasswordChecker(requestDto.Password);

            if (!passwordCheck.IsValid)
            {
                errors.AddRange(passwordCheck.Errors);
            }
            if (errors.Count > 0)
            {
                return Response<UserResponseDto>.Failure(errors: errors);
            }
            var user = _mapper.Map<User>(requestDto);
            var HashedPassword = _authService.HashPassword(requestDto.Password, user);
            user.PasswordHash = HashedPassword;
            var createdUser = await _userRepo.CreateUserAsync(user, cancellationToken);
            var result = _mapper.Map<UserResponseDto>(createdUser);
            return Response<UserResponseDto>.Success(data: result);
        }

        public async Task<Response<UserResponseDto>> ViewProfileAsyync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepo.FindByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                return Response<UserResponseDto>.Failure(errors: new List<string> { "User not found" });
            }
            var result = _mapper.Map<UserResponseDto>(user);
            return Response<UserResponseDto>.Success(result);
        }
    }
}