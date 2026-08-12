using System.Security.Claims;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Models;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ReservationController(IReservationService reservationService) : ControllerBase
    {
        private readonly IReservationService _reservationService = reservationService;
        [HttpPost]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> CreateReservationAsync(CreateReservationRequestDto createReservationRequestDto, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emailAddress = User.FindFirstValue(ClaimTypes.Name);
            if (emailAddress == null || userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }
            var result = await _reservationService.CreateReservationAsync(createReservationRequestDto, userId, emailAddress, cancellationToken);
            if (result.Errors.Any())
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = (nameof(Roles.Customer)))]
        public async Task<IActionResult> UserViewReservationsByUsernameAsync(CancellationToken cancellationToken)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username is null) return Unauthorized();
            var result = await _reservationService.GetReservationsByUsernameAsync(username, cancellationToken);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Policy = "staff")]
        [HttpGet("{username}/reservations")]
        public async Task<IActionResult> StaffViewReservationsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var result = await _reservationService.GetReservationsByUsernameAsync(username, cancellationToken);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
        [HttpPut("cancel/reservationId:guid")]
        public async Task<IActionResult> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken= default)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username is null) return Unauthorized();
            var result = await _reservationService.CancelReservationAsync(reservationId, username, cancellationToken);
            if (!result.IsSuccess)
            {
                if (result.Errors.Contains("Forbidden")) return Forbid();
                return BadRequest(result);
            }
            return Ok(result);            
        }

        [HttpGet("{reservationId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetReservationByIdAsync([FromRoute] Guid reservationId, CancellationToken cancellationToken)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username is null) return Unauthorized();
            Response<ReservationResponseDto>? result;
            if (User.IsInRole(nameof(Roles.Customer)))
            {
                result = await _reservationService.UserGetReservationByIdAsync(reservationId, username, cancellationToken);
            }
            else if (User.IsInRole(nameof(Roles.Admin)) ||
                     User.IsInRole(nameof(Roles.Manager)) ||
                     User.IsInRole(nameof(Roles.Receptionist)))
            {
                result = await _reservationService.StaffGetReservationByIdAsync(reservationId, cancellationToken);
            }
            else
            {
                return Forbid();
            }
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        [HttpGet("staff/reservations")]
        [Authorize(Policy = "staff")]
        public async Task<IActionResult> StaffGatAllReservationsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _reservationService.StaffGetAllReservationsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }

}