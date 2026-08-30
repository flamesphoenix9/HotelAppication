using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Models
{
    public class RoomTypePrice:EntityBase
    {
        [Required]
        public RoomCategory Category { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePricePerNight { get; set; }
    }
}