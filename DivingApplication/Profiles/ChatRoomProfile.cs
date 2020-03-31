using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.ChatRooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivingApplication.Entities.ManyToManyEntities;

namespace DivingApplication.Profiles
{
    public class ChatRoomProfile : Profile
    {
        public ChatRoomProfile()
        {
            CreateMap<ChatRoomForCreatingDto, ChatRoom>();
            CreateMap<ChatRoom, ChatRoomForCreatingDto>();
            CreateMap<ChatRoomUpdatingDto, ChatRoom>().ReverseMap();
            CreateMap<ChatRoom, ChatRoomOutputDto>().ForMember(
                    dest => dest.Users,
                    opt => opt.MapFrom(src => src.UserChatRooms.Select(uc => uc.User).ToList())
                ).ForMember(
                    dest => dest.Pendings,
                    opt => opt.MapFrom(src => src.UserChatRooms.Where(ucr => ucr.Role == UserChatRoom.UserChatRoomRole.Pending).Select(ucr => ucr.UserId).ToList())
                ).ForMember(
                    dest => dest.Admins,
                    opt => opt.MapFrom(src => src.UserChatRooms.Where(ucr => ucr.Role == UserChatRoom.UserChatRoomRole.Admin).Select(ucr => ucr.UserId).ToList())
                ).ForMember(
                    dest => dest.OwnerId,
                    opt => opt.MapFrom(src => src.UserChatRooms.SingleOrDefault(ucr => ucr.Role == UserChatRoom.UserChatRoomRole.Owner).UserId)
                );

        }

    }
}
