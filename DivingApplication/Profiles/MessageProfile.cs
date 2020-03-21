using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Message;

namespace DivingApplication.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<MessageForCreatingDto, Message>().ForMember(
                    dest => dest.MessageType,
                    opt => opt.MapFrom(src => (MessageContentType)Enum.Parse(typeof(MessageContentType), src.MessageType))
                );


            CreateMap<Message, MessageForCreatingDto>().ForMember(
                    dest => dest.MessageType,
                    opt => opt.MapFrom(src => src.MessageType.ToString())
                );

            CreateMap<Message, MessageOutputDto>().ForMember(
                    dest => dest.MessageType,
                    opt => opt.MapFrom(src => src.MessageType.ToString())
                ).ForMember(
                    dest => dest.ReadBy,
                    opt => opt.MapFrom(src => src.ReadBy.Select(r => r.UserId))
                ).ForMember(
                    dest => dest.LikedBy,
                    opt => opt.MapFrom(src => src.LikedBy.Select(r => r.UserId))
                );

        }
    }
}
