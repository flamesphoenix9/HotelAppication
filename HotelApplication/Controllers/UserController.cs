using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelApplication.Dtos;
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
    }
}