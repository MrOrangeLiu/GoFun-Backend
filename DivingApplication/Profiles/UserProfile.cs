using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.User;

namespace DivingApplication.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserForCreatingDto, User>().ForMember(
                dest => dest.UserGender,
                opt => opt.MapFrom(src => src.UserGender == 0 ? Gender.Female : Gender.Male)
                );

        }
    }
}
