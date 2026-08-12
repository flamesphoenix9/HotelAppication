using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelApplication.Dtos;
using HotelApplication.Models;

namespace HotelApplication.MappingProfiles.UserProfiles
{
    public class CreateUserProfile:Profile
    {
        public CreateUserProfile()
        {
            CreateMap<CreateUserRequestDto, User>().ForMember(u => u.PasswordHash, opt => opt.Ignore());
            CreateMap<AdminCreateUserRequestDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserResponseDto>().ForMember(dest => dest.UserId, m => m.MapFrom(s => s.Id)).ForMember(dest => dest.UserRole, m => m.MapFrom(s => s.Role));
            CreateMap<User, AdminViewUserResponseDto>().ForSourceMember(s => s.PasswordHash, opt => opt.DoNotValidate());
        }
    }
}