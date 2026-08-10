using HotelApplication.Dtos.RoomDtos;
using HotelApplication.Repos;
using HotelApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApplication.Controllers
{
    [Authorize(Policy ="staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController(IRoomService roomService) : ControllerBase
    {
        private readonly IRoomService _roomService = roomService;
        [HttpGet]
        public async Task<IActionResult> GetAllRoomsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetAllRoomsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(CreateRoomRequestDto createRoomRequestDto, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.CraeteRoomAsync(createRoomRequestDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("roomId:guid")]
        public async Task<IActionResult> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetRoomByIdAsync(roomId, cancellationToken);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
        [HttpPut("roomId:guid")]
        public async Task<IActionResult> UpdateRoomStatusAsync(UpdateRoomStatusDto updateRoomStatusDto, CancellationToken cancellationToken = default)
        {
            var result = await _roomService.UpdateRoomStatusAsync(updateRoomStatusDto, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("/available")]
        public async Task<IActionResult> GetAvailableRoomsAsync(CancellationToken cancellationToken = default)
        {
            var result = await _roomService.GetAvailableRoomsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
        
    }
}