using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    public class RoomDateAllocation : EntityBase
    {
        [Required]
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public DateOnly Date { get; set; }
        public Guid ReservationGroupId { get; set; }
        public Guid ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
    }
}