using HotelApplication.Dtos.RoomDtos;
using HotelApplication.Repos;
using HotelApplication.Services;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController(IRoomService roomService) : ControllerBase
    {
        private readonly IRoomService _roomService = roomService;
        [HttpGet]
        [Authorize(Policy = "staff")]
        [ProducesResponseType(typeof(Response<IReadOnlyCollection<RoomResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllRoomsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetAllRoomsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Policy ="management")]
        [ProducesResponseType(typeof(Response<RoomResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoomAsync(CreateRoomRequestDto createRoomRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.CraeteRoomAsync(createRoomRequestDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("roomId:guid")]
        [Authorize(Policy = "staff")]
        [ProducesResponseType(typeof(Response<RoomResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetRoomByIdAsync(roomId, cancellationToken);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
        [HttpPut("roomId:guid")]
        [Authorize(Policy = "housekeeping")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoomStatusAsync(UpdateRoomStatusDto updateRoomStatusDto, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.UpdateRoomStatusAsync(updateRoomStatusDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("/available")]
        [Authorize(Policy = "front-desk")]
        [ProducesResponseType(typeof(Response<IReadOnlyCollection<RoomResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAvailableRoomsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetAvailableRoomsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        
    }
}