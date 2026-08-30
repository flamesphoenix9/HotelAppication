using System.Security.Claims;
using HotelApplication.Dtos;
using HotelApplication.Models;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userservice = userService;
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<UserResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUserAsync(CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            var result = await _userservice.RegisterAsync(createUserRequestDto, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var result = await _userservice.LoginAsync(loginRequestDto, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [Authorize]
        [HttpGet("view-profile")]
        [ProducesResponseType(typeof(Response<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ViewProfileAsync(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }
            var result = await _userservice.ViewProfileAsyync(userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("admin/register-user")]
        [ProducesResponseType(typeof(Response<AdminViewUserResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdminRegisterUserAsync(AdminCreateUserRequestDto adminCreateUserRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await _userservice.AdminRegisterUserAsync(adminCreateUserRequestDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        
        [Authorize(Policy ="management")]
        [HttpPut("admin/user/{userId:guid}/activate-account")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActivateuserAccountAsyc(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await _userservice.AddminActivateUserAsync(userId, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Policy ="management")]
        [HttpPut("admin/user/{userId:guid}/deactivate-account")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]        
        public async Task<IActionResult> DeactivateUserAccountAsyc(Guid userId, CancellationToken cancellationToken= default)
        {
            var result = await _userservice.AdminDeactivateUserAsync(userId, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}