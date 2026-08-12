using System.Security.Claims;
using HotelApplication.Dtos;
using HotelApplication.Models;
using HotelApplication.Services;
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
        public async Task<IActionResult> RegisterUserAsync(CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            var result = await _userservice.RegisterAsync(createUserRequestDto, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("login")]
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
        public async Task<IActionResult> ViewProfileAsync(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }
            var result = await _userservice.ViewProfileAsyync(userId, cancellationToken);
            return Ok(result);
        }
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost("admin/register-user")]
        public async Task<IActionResult> AdminRegisterUserAsync(AdminCreateUserRequestDto adminCreateUserRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await _userservice.AdminRegisterUserAsync(adminCreateUserRequestDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut("admin/user/{userId:guid}/activate-account")]
        public async Task<IActionResult> ActivateuserAccountAsyc(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await _userservice.AddminActivateUserAsync(userId, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPut("admin/user/{userId:guid}/deactivate-account")]
        public async Task<IActionResult> DeactivateuserAccountAsyc(Guid userId, CancellationToken cancellationToken= default)
        {
            var result = await _userservice.AdminDeactivateUserAsync(userId, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}