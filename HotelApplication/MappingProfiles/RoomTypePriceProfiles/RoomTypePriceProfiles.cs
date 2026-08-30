using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos.RoomTypePriceDtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.RoomTypePriceProfiles
{
    public class RoomTypePriceProfiles:Profile
    {
        public RoomTypePriceProfiles()
        {
            CreateMap<CreateRoomTypePriceRequestDto, RoomTypePrice>();
            CreateMap<RoomTypePrice, RoomTypePriceResponseDto>();
        }
    }
}