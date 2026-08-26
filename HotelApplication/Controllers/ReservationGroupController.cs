using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelApplication.Dtos.PaymentDtos;
using HotelApplication.Dtos.ReservationGroupDtos;
using HotelApplication.Models;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationGroupController(IReservationGroupService reservationGroupService) : ControllerBase
    {
        private readonly IReservationGroupService _reservationGroupService = reservationGroupService;

        [HttpPost]
        [Authorize(Roles = nameof(Roles.Customer))]
        [ProducesResponseType(typeof(Response<ReservationGroupResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationGroupRequestDto createReservationGroupRequestDto, CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var response = await _reservationGroupService.CreateReservationGroupAsync(userId, createReservationGroupRequestDto, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("{reservationGroupId:guid}/initialize-payment")]
        [Authorize(Roles = nameof(Roles.Customer))]
        [ProducesResponseType(typeof(Response<PaymentInitializeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> InitializePaymentAsync(
            [FromRoute] Guid reservationGroupId,
            CancellationToken cancellationToken = default)
        {
            var emailClaim = User.FindFirstValue(ClaimTypes.Email);
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(emailClaim) || !MailAddress.TryCreate(emailClaim, out var email))
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            var response = await _reservationGroupService.InitializePaymentAsync(reservationGroupId, email: email.ToString(), userId: userId, cancellationToken);
            if (!response.IsSuccess)
            {
                if (response.Errors.Contains("403"))
                {
                    return Forbid("You do not have the necessary perms");
                }
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("verify-payment/{reservationGroupCode}")]
        [ProducesResponseType(typeof(Response<ReservationGroupResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerifyPaymentAsync(
            [FromRoute] string reservationGroupCode,
            CancellationToken cancellationToken = default)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            bool isStaff = User.IsInRole(nameof(Roles.Receptionist)) || User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.Manager));
            var response = await _reservationGroupService.VerifyAndCnfirmPaymentAsync(reservationGroupCode, userId, isStaff, cancellationToken);
            if (!response.IsSuccess)
            {
                if (response.Errors.Contains("403"))
                {
                    return Forbid("You do not have the necessary perms");
                }
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("{reservationGroupId:guid}")]
        [ProducesResponseType(typeof(ReservationGroupResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetReservationGroupById(
            [FromRoute] Guid reservationGroupId,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            bool isStaff = User.IsInRole(nameof(Roles.Receptionist)) || User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.Manager));
            var response = await _reservationGroupService.GetReservationGroupByIdAsync(
                reservationGroupId,
                userId,
                isStaff,
                cancellationToken);

            if (!response.IsSuccess)
            {
                if (response.Errors.Contains("403"))
                {
                    return Forbid("You do not have the necessary perms");
                }
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpGet("by-code/{reservationGroupCode}")]
        [ProducesResponseType(typeof(Response<ReservationGroupResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetReservationGroupByCode(
            [FromRoute] string reservationGroupCode,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }
            bool isStaff = User.IsInRole(nameof(Roles.Receptionist)) || User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.Manager));
            var response = await _reservationGroupService.GetReservationGroupByCodeAsync(
                reservationGroupCode,
                userId,
                isStaff,
                cancellationToken);

            if (!response.IsSuccess)
            {
                if (response.Errors.Contains("403"))
                {
                    return Forbid("You do not have the necessary perms");
                }
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}