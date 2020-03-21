using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.ChatRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Profiles
{
    public class ChatRoomProfile : Profile
    {
        public ChatRoomProfile()
        {
            CreateMap<ChatRoomForCreatingDto, ChatRoom>();
            CreateMap<ChatRoom, ChatRoomForCreatingDto>().ForMember(
                    dest => dest.UserIds,
                    opt => opt.MapFrom(src => src.UserChatRooms.Select(uc => uc.UserId))
                );
            CreateMap<ChatRoomUpdatingDto, ChatRoom>().ReverseMap();
            CreateMap<ChatRoom, ChatRoomOutputDto>().ForMember(
                    dest => dest.Users,
                    opt => opt.MapFrom(src => src.UserChatRooms.Select(uc => uc.User))
                );

        }

    }
}
