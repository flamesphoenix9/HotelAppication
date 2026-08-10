using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;

namespace HotelApplication.Dtos.RoomDtos
{
    public record CreateRoomRequestDto
    {
        public string RoomNumber { get; init; } = string.Empty;
        [EnumDataType(typeof(RoomCategory), ErrorMessage ="Invalid room category")]
        public string RoomCategory { get; init; } = string.Empty;
    }
}