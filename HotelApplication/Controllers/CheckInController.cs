using System.Security.Claims;
using HotelApplication.Dtos.CheckInDtos;
using HotelApplication.Models;
using HotelApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController(ICheckInService checkInService) : ControllerBase
    {
        private readonly ICheckInService _checkInService = checkInService;
        [Authorize(Roles = nameof(Roles.Receptionist))]
        [HttpPost]
        public async Task<IActionResult> CreateCheckInAsync(CreateCheckInRequestDto createCheckInRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await _checkInService.CreateCheckInAsync(createCheckInRequestDto, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Roles = nameof(Roles.Receptionist))]
        [HttpPut("complete-checkout/{checkInId:guid}")]
        public async Task<IActionResult> CompleteCheckOutAsync(Guid checkInId, CancellationToken cancellationToken = default)
        {
            var result = await _checkInService.CompleteCheckOutAsync(checkInId, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("users/{username}")]
        public async Task<IActionResult> GetUserCheckInsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var role = Enum.TryParse<Roles>(User.FindFirstValue(ClaimTypes.Role), true, out var userRole);
            if (!role)
            {
                return Unauthorized();
            }
            string actualUsername;
            if (userRole == Roles.Customer)
            {
                actualUsername = User.FindFirstValue(ClaimTypes.Name)!;
            }
            else
            {
                actualUsername = username;
            }
    
            var result = await _checkInService.GetCheckInsByUsernameAsync(actualUsername, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Policy = "staff")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveCheckInsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _checkInService.GetAllActiveCheckInsAsync(cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{checkInId:Guid}")]
        public async Task<IActionResult> GetCheckInByIdAsync(Guid checkInId, CancellationToken cancellationToken = default)
        {
            var role = Enum.TryParse<Roles>(User.FindFirstValue(ClaimTypes.Role), true, out var userRole);
            if (!role)
            {
                return Unauthorized();
            }
            string username;
            if (userRole == Roles.Customer)
            {
                username = User.FindFirstValue(ClaimTypes.Name)!;
            }
            else
            {
                username = string.Empty;
            }
            var result = await _checkInService.GetCheckInByIdAsync(checkInId, userRole, username, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize(Policy = "staff")]
        [HttpGet("room/{roomNumber}")]
        public async Task<IActionResult> GetAllRoomCheckInHistoryAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var result = await _checkInService.GetRoomCheckInHistoryAsync(roomNumber, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize(Policy = "staff")]
        [HttpGet("active/{roomNumber}")]
        public async Task<IActionResult> GetActiveCheckInByRoomNumberAsync(string roomNumber, CancellationToken cancellationToken = default)
        {
            var result = await _checkInService.GetActiveCheckInByRoomNumberAsync(roomNumber, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    
    }
}