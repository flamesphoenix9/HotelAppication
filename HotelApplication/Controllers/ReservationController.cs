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
        [Authorize(Roles = nameof(Roles.Customer))]
        [HttpPost]
        public async Task<IActionResult> CreateReservationAsync(CreateReservationRequestDto createReservationRequestDto, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emailAddress = User.FindFirstValue(ClaimTypes.Email);
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
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> ViewAllReservationsAsync(CancellationToken cancellationToken)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username is null) return Unauthorized();
            var result = await _reservationService.UserViewAllReservationsAsync(username, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{reservationId:guid}")]
        public async Task<IActionResult> GetReservationAsync([FromRoute] Guid reservationId, CancellationToken cancellationToken)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (username is null ) return Unauthorized();
            Response<ReservationResponseDto>? result;
            if (User.IsInRole(nameof(Roles.Customer)))
            {
                result = await reservationService.UserViewReservationAsync(reservationId, username, cancellationToken);
            }
            else if (User.IsInRole(nameof(Roles.Admin)) || 
                     User.IsInRole(nameof(Roles.Manager)) || 
                     User.IsInRole(nameof(Roles.Receptionist)))
            {
                result = await reservationService.StaffViewReservationAsync(reservationId, cancellationToken);
            }
            else
            {
                return Forbid();
            }
            return Ok(result);
        }
    }

}