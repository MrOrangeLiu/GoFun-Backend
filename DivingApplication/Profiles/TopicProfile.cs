using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Topic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Profiles
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<TopicForCreatingDto, Topic>().ReverseMap();

            CreateMap<TopicUpdatingDto, Topic>().ReverseMap();

            CreateMap<Topic, TopicOutputDto>().ForMember(
                dest => dest.TopicPostsCount,
                opt => opt.MapFrom(src => src.TopicPosts.Count)
                );
        }
    }
}
