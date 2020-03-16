using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models;
using DivingApplication.Models.Users;
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
                opt => opt.MapFrom(src => (Gender)Enum.Parse(typeof(Gender), src.UserGender))
                );


            CreateMap<UserUpdatingDto, User>().ForMember(
                dest => dest.UserGender,
                opt => opt.MapFrom(src => (Gender)Enum.Parse(typeof(Gender), src.UserGender))
                );

            CreateMap<User, UserUpdatingDto>().ForMember(
                dest => dest.UserGender,
                opt => opt.MapFrom(src => src.UserGender.ToString())
                );


            CreateMap<User, UserOutputDto>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.ToString()))
                .ForMember(dest => dest.UserGender, opt => opt.MapFrom(src => src.UserGender.ToString()))
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers.Select(f => f.FollowerId).ToList()))
                .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following.Select(f => f.FollowingId).ToList()))
                .ForMember(dest => dest.OwningPosts, opt => opt.MapFrom(src => src.OwningPosts.Select(p => p.Id).ToList()))
                .ForMember(dest => dest.UserChatRooms, opt => opt.MapFrom(src => src.UserChatRooms.Select(c => c.ChatRoomId).ToList()))
                .ForMember(dest => dest.SavePosts, opt => opt.MapFrom(src => src.SavePosts.Select(p => p.PostId).ToList()))
                .ForMember(dest => dest.LikePosts, opt => opt.MapFrom(src => src.LikePosts.Select(p => p.PostId).ToList()))
                .ForMember(dest => dest.OwingServiceInfos, opt => opt.MapFrom(src => src.OwningServiceInfos.Select(s => s.Id).ToList()));



            CreateMap<User, UserBriefOutputDto>()
              .ForMember(dest => dest.CoachInfoId, opt => opt.MapFrom(src => src.CoachInfo.Id))
              .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.ToString()))
              .ForMember(dest => dest.UserGender, opt => opt.MapFrom(src => src.UserGender.ToString()))
              .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
              .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Following.Count))
              .ForMember(dest => dest.OwningPostsCount, opt => opt.MapFrom(src => src.OwningPosts.Count))
              .ForMember(dest => dest.OwningServiceInfosCount, opt => opt.MapFrom(src => src.OwningServiceInfos.Count));


        }
    }
}
