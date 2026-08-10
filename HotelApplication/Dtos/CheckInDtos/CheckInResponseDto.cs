using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Dtos.CheckInDtos
{
    public record CheckInResponseDto
    {
        //  CheckIn data
        public Guid CheckInId{ get; init; }
        public DateTime CheckInTime { get; init; }
        public DateTime? CheckOutTime { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public bool IsCheckInActive{ get; init; }
        //  User data
        public string FullName { get; init; } = string.Empty;
        public string EmailAddress { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        //  Room data
        public Guid RoomId { get; init; }
        public string RoomNumber { get; init; }= string.Empty;
        public string RoomCategory { get; init; } = string.Empty;

    }
}