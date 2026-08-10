using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.CheckInDtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.CheckInProfiles
{
    public class CheckInProfiles:Profile
    {
        public CheckInProfiles()
        {
            CreateMap<CheckIn, CreateCheckInResponseDto>().ForMember(dest => dest.FullName, m => m.MapFrom(s => s.User.FullName));
            //Profile 2
            CreateMap<CheckIn, CheckInResponseDto>().ForMember(dest => dest.FullName, m => m.MapFrom(s => s.User.FullName)).ForMember(dest => dest.EmailAddress, m => m.MapFrom(s => s.User.EmailAddress)).ForMember(dest => dest.Username, m => m.MapFrom(s => s.User.Username)).ForMember(dest => dest.RoomCategory, m => m.MapFrom(s => s.Room.RoomCategory)).ForMember(dest => dest.CheckInId, m => m.MapFrom(s => s.Id));
            
        }
    }
}