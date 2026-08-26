using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Dtos.ReservationGroupDtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.ReservationGroupProfiles
{
    public class ReservationGroupProfiles:Profile
    {
        public ReservationGroupProfiles()
        {
            CreateMap<Reservation, ReservationResponseDto>().ForMember(d => d.RoomNumber, m => m.MapFrom(s => s.AssignedRoom != null ? s.AssignedRoom.RoomNumber : string.Empty));
            CreateMap<ReservationGroup, ReservationGroupResponseDto>().ForMember(d => d.Username, m => m.MapFrom(s => s.User != null ? s.User.Username : string.Empty));
        }
    }
}