using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelApplication.Utilities
{
    public static class ReservationGroupUtilities
    {
        public static string GenerateGroupCode()
        {
            string datepart = DateTime.UtcNow.ToString("ddMmyyyy");
            string randompart = Guid.NewGuid().ToString()[..6].ToUpperInvariant();
            return $"RES-{datepart}-{randompart}";
        }
    }
}