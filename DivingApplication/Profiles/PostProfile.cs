using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Post;

namespace DivingApplication.Profiles
{
    public class PostProfile : Profile
    {


        public PostProfile()
        {
            CreateMap<PostForCreatingDto, Post>()
                .ForMember(
                    dest => dest.PostContentType,
                    opt => opt.MapFrom(src => (ContentType)Enum.Parse(typeof(ContentType), src.PostContentType))
                ).ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => String.Join(urlSplittor, src.ContentURL))
                );


            CreateMap<Post, PostForCreatingDto>().ForMember(
                    dest => dest.PostContentType,
                    opt => opt.MapFrom(src => src.PostContentType.ToString())
                ).ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => src.ContentURL.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                );


            CreateMap<PostUpdatingDto, Post>().ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => String.Join(urlSplittor, src.ContentURL))
                );

            CreateMap<Post, PostUpdatingDto>().ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => src.ContentURL.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                );

            CreateMap<Post, PostOutputDto>().ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => src.ContentURL.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.PostContentType,
                    opt => opt.MapFrom(src => src.PostContentType.ToString())
                ).ForMember(
                    dest => dest.PostLikedCount,
                    opt => opt.MapFrom(src => src.PostLikedBy.Count)
                ).ForMember(
                    dest => dest.PostSavedCount,
                    opt => opt.MapFrom(src => src.PostSavedBy.Count)
                ).ForMember(
                    dest => dest.CommentCount,
                    opt => opt.MapFrom(src => src.Comments.Count)
                ).ForMember(
                    dest => dest.TaggedUsers,
                    opt => opt.MapFrom(src => src.TaggedUsers.Select(tu => tu.User))
                ).ForMember(
                    dest => dest.PostTopics,
                    opt => opt.MapFrom(src => src.PostTopics.Select(pt => pt.Topic))
                );

            CreateMap<Post, PostPrviewOutputDto>().ForMember(
                    dest => dest.ContentURL,
                    opt => opt.MapFrom(src => src.ContentURL.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.PostContentType,
                    opt => opt.MapFrom(src => src.PostContentType.ToString())
                ).ForMember(
                    dest => dest.PostLikedCount,
                    opt => opt.MapFrom(src => src.PostLikedBy.Count)
                ).ForMember(
                    dest => dest.PostSavedCount,
                    opt => opt.MapFrom(src => src.PostSavedBy.Count)
                ).ForMember(
                    dest => dest.CommentCount,
                    opt => opt.MapFrom(src => src.Comments.Count)
                ).ForMember(
                    dest => dest.TaggedUsers,
                    opt => opt.MapFrom(src => src.TaggedUsers.Select(tu => tu.User))
                ).ForMember(
                    dest => dest.PostTopics,
                    opt => opt.MapFrom(src => src.PostTopics.Select(pt => pt.Topic))
                ); ;

        }
    }
}
