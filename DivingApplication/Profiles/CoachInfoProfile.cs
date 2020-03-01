using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.CoachInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Profiles
{
    public class CoachInfoProfile : Profile
    {
        public CoachInfoProfile()
        {
            CreateMap<CoachInfoForCreatingDto, CoachInfo>().ForMember(
                    dest => dest.LocationImageUrls,
                    opt => opt.MapFrom(src => string.Join(CoachInfo.urlSplittor, src.LocationImageUrls))
                ).ForMember(
                    dest => dest.SelfieUrls,
                    opt => opt.MapFrom(src => string.Join(CoachInfo.urlSplittor, src.SelfieUrls))
                );

            CreateMap<CoachInfo, CoachInfoForCreatingDto>()
                .ForMember(dest => dest.LocationImageUrls,
                           opt => opt.MapFrom(src => src.LocationImageUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(dest => dest.SelfieUrls,
                           opt => opt.MapFrom(src => src.SelfieUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)));

            CreateMap<CoachInfoUpdatingDto, CoachInfo>()
                .ForMember(
                   dest => dest.LocationImageUrls,
                   opt => opt.MapFrom(src => string.Join(CoachInfo.urlSplittor, src.LocationImageUrls))
                ).ForMember(
                    dest => dest.SelfieUrls,
                    opt => opt.MapFrom(src => string.Join(CoachInfo.urlSplittor, src.SelfieUrls))
                );

            CreateMap<CoachInfo, CoachInfoUpdatingDto>()
                .ForMember(dest => dest.LocationImageUrls,
                           opt => opt.MapFrom(src => src.LocationImageUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(dest => dest.SelfieUrls,
                           opt => opt.MapFrom(src => src.SelfieUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)));


            CreateMap<CoachInfo, CoachInfoOutputDto>()
                .ForMember(dest => dest.LocationImageUrls,
                           opt => opt.MapFrom(src => src.LocationImageUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(dest => dest.SelfieUrls,
                           opt => opt.MapFrom(src => src.SelfieUrls.Split(new[] { CoachInfo.urlSplittor }, StringSplitOptions.RemoveEmptyEntries)));

        }
    }
}
