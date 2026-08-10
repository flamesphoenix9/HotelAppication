using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.RoomDtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.RoomProfiles
{
    public class RoomProfiles:Profile
    {
        public RoomProfiles()
        {
            CreateMap<Room, RoomResponseDto>().ForMember(dest => dest.Status, m => m.MapFrom(s => s.Status.ToString())).ForMember(dest => dest.RoomCategory, m => m.MapFrom(s => s.RoomCategory.ToString()));
            CreateMap<CreateRoomRequestDto, Room>().ForMember(dest => dest.RoomCategory, m => m.MapFrom(s => (RoomCategory) Enum.Parse(typeof(RoomCategory), s.RoomCategory))).ForMember(dest=>dest.RoomNumber, m=>m.MapFrom(s=>s.RoomNumber.ToUpperInvariant()));
        }
    }
}