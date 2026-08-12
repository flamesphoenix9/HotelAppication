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
            if (!_authService.IsValidEmail(requestDto.EmailAddress))
            {
                return Response<LoginResponseDto>.Failure(new List<string> { $"{requestDto.EmailAddress} is not a valid email Address" });
            }
            var user = await _userRepo.FindByEmailAsync(requestDto.EmailAddress, cancellationToken);
            if (user is null)
            {
                return Response<LoginResponseDto>.Failure(new List<string>() { $"user with email {requestDto.EmailAddress} not found" });
            }
            if (user.IsDeleted) return Response<LoginResponseDto>.Failure(["User account deactivated"]);
            var verifyPassword = _authService.VerifyPassword(user, requestDto.Password);
            if (!verifyPassword) return Response<LoginResponseDto>.Failure(new List<string> { "Incorrect Password" });
            var token = _authService.GenerateJsonWebToken(user);
            var data = new LoginResponseDto { AccessToken = token };
            return Response<LoginResponseDto>.Success(data);
        }

        public async Task<Response<UserResponseDto>> RegisterAsync(CreateUserRequestDto requestDto, CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            if (!_authService.IsValidEmail(requestDto.EmailAddress))
            {
                errors.Add($"{requestDto.EmailAddress} is not a valid email Address");
            }
            var emailAddress = requestDto.EmailAddress;
            var username = requestDto.Username;
            var userExists = await _userRepo.CheckUniqueEmailAndUsername(emailAddress, username, cancellationToken);
            if (userExists?.Any(u => string.Equals(u.Normalised_Email, emailAddress, StringComparison.OrdinalIgnoreCase))==true)
            {
                errors.Add($"User with email address {emailAddress} already exists");
            }
            if (userExists?.Any(u => string.Equals(u.Normalised_Username ,username, StringComparison.OrdinalIgnoreCase))==true)
            {
                errors.Add($"User with username {username} already exists");
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

        public async Task<Response<AdminViewUserResponseDto>> AdminRegisterUserAsync(AdminCreateUserRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();
            if (!Enum.TryParse<Roles>(requestDto.Role, true, out var userRole))
            {
                errors.Add($"{requestDto.Role} is not a valid role");
            }
            if (!_authService.IsValidEmail(requestDto.EmailAddress))
            {
                errors.Add($"{requestDto.EmailAddress} is not a valid email Address");
            }
            var emailAddress = requestDto.EmailAddress;
            var username = requestDto.Username;
            var userExists = await _userRepo.CheckUniqueEmailAndUsername(emailAddress, username, cancellationToken);
            if (userExists?.Any(u => string.Equals(u.Normalised_Email, emailAddress, StringComparison.OrdinalIgnoreCase)) == true)
            {
                errors.Add($"User with email address {emailAddress} already exists");
            }
            if (userExists?.Any(u => string.Equals(u.Normalised_Username, username, StringComparison.OrdinalIgnoreCase)) == true)
            {
                errors.Add($"User with username {username} already exists");
            }
            var passwordCheck = _authService.PasswordChecker(requestDto.Password);
            if (!passwordCheck.IsValid)
            {
                errors.AddRange(passwordCheck.Errors);
            }
            if (errors.Count > 0)
            {
                return Response<AdminViewUserResponseDto>.Failure(errors: errors);
            }
            var user = _mapper.Map<User>(requestDto);
            var HashedPassword = _authService.HashPassword(requestDto.Password, user);
            user.PasswordHash = HashedPassword;
            var createdUser = await _userRepo.CreateUserAsync(user, cancellationToken);
            var result = _mapper.Map<AdminViewUserResponseDto>(createdUser);
            return Response<AdminViewUserResponseDto>.Success(data: result);
        }

        public async Task<Response<IReadOnlyCollection<AdminViewUserResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepo.AdminGetAllUsersAsync(cancellationToken);
            var result = users.Select(u => _mapper.Map<AdminViewUserResponseDto>(u)).ToList();
            return Response<IReadOnlyCollection<AdminViewUserResponseDto>>.Success(result);
        }

        public async Task<Response<bool>> AdminDeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.FindByIdAsync(userId, cancellationToken);
            if (user is null) return Response<bool>.Failure([$"user with Id {userId} not found"]);
            if (user.IsDeleted) return Response<bool>.Failure([$"User {user.EmailAddress} already deactivated "]);
            user.DeactivateUserAccount();
            await _userRepo.UpdateUsersAsync(user, cancellationToken);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> AddminActivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.FindByIdAsync(userId, cancellationToken);
            if (user is null) return Response<bool>.Failure([$"user with Id {userId} not found"]);
            if (!user.IsDeleted) return Response<bool>.Failure([$"User {user.EmailAddress} already activated "]);
            user.ActivateUserAccount();
            await _userRepo.UpdateUsersAsync(user, cancellationToken);
            return Response<bool>.Success(true);
        }
    }
}