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
            CreateMap<CreateReservationRequestDto, Reservation>();
            // CreateMap<Reservation, CreateReservationResponseDto>().ForMember(d => d.RequestedRoomCategory, m => m.MapFrom(s => s.ReservationCategory.ToString()));
            CreateMap<Reservation, ReservationResponseDto>().ForMember(d => d.ReservationCategory, m => m.MapFrom(s => s.ReservationCategory.ToString())).ForMember(dest => dest.ReservationStatus, m => m.MapFrom(s => s.ReservationStatus.ToString()));
            CreateMap<Reservation, StaffReservationResponseDto>().ForMember(dest => dest.RequestedRoomCategory, m => m.MapFrom(s => s.ReservationCategory.ToString())).ForMember(dest => dest.ReservationStatus, m => m.MapFrom(s => s.ReservationStatus.ToString()));
        }
    }
}