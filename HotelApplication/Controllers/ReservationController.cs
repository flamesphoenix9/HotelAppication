using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Models;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController(IReservationService reservationService, ILogger<ReservationController> logger) : ControllerBase
    {
        private readonly IReservationService _reservationService = reservationService;
        private readonly ILogger<ReservationController> _logger = logger;

        [HttpGet("{reservationId:guid}")]
        [ProducesResponseType(typeof(Response<ReservationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReservationById([FromRoute] Guid reservationId, CancellationToken cancellationToken = default)
        {
            var isStaff = User.IsInRole(nameof(Roles.Manager)) || User.IsInRole(nameof(Roles.Receptionist)) || User.IsInRole(nameof(Roles.Admin));
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var response = await _reservationService.GetReservationByIdAsync(reservationId, userId, isStaff, cancellationToken);
            if (!response.IsSuccess)
            {
                if (response.Errors.Contains("403"))
                {
                    return Forbid();
                }
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Policy = "front-desk")]
        [HttpPut("reservationId:guid/checkIn")]
        [ProducesResponseType(typeof(Response<ReservationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckInReservationAsync([FromRoute] Guid reservationId, [FromBody] ReservationCheckInRequestDto reservationCheckInRequestDto, CancellationToken cancellationToken = default)
        {
            var staffData = User.FindFirstValue(ClaimTypes.UserData);
            var response = await _reservationService.CheckInReservationAsyc(reservationId, reservationCheckInRequestDto, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            _logger.LogInformation("{Reservation} checked in by {Staff}", reservationId, staffData);
            return Ok(response);
        }

        [Authorize(Policy = "front-desk")]
        [HttpPut("reservationId:guid/checkOut")]
        [ProducesResponseType(typeof(Response<ReservationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckOutReservationAsync([FromRoute] Guid reservationId, CancellationToken cancellationToken = default)
        {
            var staffData = User.FindFirstValue(ClaimTypes.UserData);
            var response = await _reservationService.CheckOutReservatonAsync(reservationId, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            _logger.LogInformation("{Reservation} checked out by {Staff}", reservationId, staffData);
            return Ok(response);
        }
        
        [Authorize(Policy ="front-desk")]
        [HttpPut("reservationId:guid/assign-room")]
        [ProducesResponseType(typeof(Response<ReservationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAssignedRoomAsync([FromRoute] Guid reservationId, CancellationToken cancellationToken= default)
        {
            var staffData = User.FindFirstValue(ClaimTypes.UserData);
            var response = await _reservationService.UpdateAssignedRoomAsync(reservationId, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            _logger.LogInformation("{Reservation} room reassigned by {Staff}", reservationId, staffData);
            return Ok(response);
        }
    }
}