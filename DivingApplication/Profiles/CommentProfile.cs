using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Profiles
{
    public class CommentProfile : Profile
    {

        public CommentProfile()
        {
            CreateMap<CommentForCreatingDto, Comment>().ReverseMap();
            CreateMap<Comment, CommentUpdatingDto>().ReverseMap();
            CreateMap<Comment, CommentOutputDto>();
        }
    }
}
