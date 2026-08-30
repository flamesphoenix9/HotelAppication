using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelApplication.Dtos.RoomTypePriceDtos;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy ="management")]
    public class RoomTypePriceController(ILogger logger,
                                        IRoomTypePriceService roomTypePriceService) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly IRoomTypePriceService _roomTypePriceService = roomTypePriceService;

        [HttpPost]
        [ProducesResponseType(typeof(Response<RoomTypePriceResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoomTypePriceAsync(CreateRoomTypePriceRequestDto createRoomTypePriceRequestDto, CancellationToken cancellationToken)
        {
            var staffData = User.FindFirstValue(ClaimTypes.UserData);
            var response = await _roomTypePriceService.CreateRoomTypePrice(createRoomTypePriceRequestDto, cancellationToken);
            if (!response.IsSuccess)
            {
                _logger.LogCritical("Failed: {Staff} tried to create new Room type pricing", staffData);
                return BadRequest(response);
            }
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<IReadOnlyCollection<RoomTypePriceResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllRoomPricesAsync(CancellationToken cancellationToken)
        {
            var response = await _roomTypePriceService.GetAllRoomTypePrices(cancellationToken);
            if (!response.IsSuccess)
            {
               return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPut("{roomTypePriceId:guid}/update")]
        [ProducesResponseType(typeof(Response<RoomTypePriceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoomPricingAsync([FromRoute] Guid roomTypePriceId, [FromBody] EditRoomTypeRequestDto editRoomTypeRequestDto, CancellationToken cancellationToken = default)
        {
            var staffData = User.FindFirstValue(ClaimTypes.UserData);
            var response = await _roomTypePriceService.EditRoomTypePriceAsync(roomTypePriceId, editRoomTypeRequestDto, cancellationToken);
            if (!response.IsSuccess)
            {
                _logger.LogCritical("Failed: {Staff} tried to update room type price {roomTypePriceId}", staffData, roomTypePriceId);
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("{roomTypePriceId:guid}/update")]
        [ProducesResponseType(typeof(Response<RoomTypePriceResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRoomPricingByIdAsync([FromRoute]Guid roomTypePriceId, CancellationToken cancellationToken= default)
        {
            var response = await _roomTypePriceService.GetRoomTypePriceByIdAsync(roomTypePriceId, cancellationToken);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}