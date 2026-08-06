using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.ReservationDtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.ReservationProfiles
{
    public class ReservationProfiles:Profile
    {
        public ReservationProfiles()
        {
            CreateMap<Reservation, CreateReservationResponseDto>().ForMember(d => d.RequestedRoomCategory, m => m.MapFrom(s => s.RequestedRoomCategory.ToString()));
            CreateMap<Reservation, ReservationResponseDto>().ForMember(d => d.RequestedRoomCategory, m => m.MapFrom(s => s.RequestedRoomCategory.ToString())).ForMember(dest => dest.FullName, m => m.MapFrom(s => s.User.FullName)).ForMember(dest => dest.ReservationStatus, m => m.MapFrom(s => s.ReservationStatus.ToString()));
            CreateMap<Reservation, StaffReservationResponseDto>().ForMember(dest => dest.RequestedRoomCategory, m => m.MapFrom(s => s.RequestedRoomCategory.ToString())).ForMember(dest => dest.FullName, m => m.MapFrom(s => s.User.FullName)).ForMember(dest => dest.ReservationStatus, m => m.MapFrom(s => s.ReservationStatus.ToString()));
        }
    }
}