using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.ServiceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.ServiceInfo;

namespace DivingApplication.Profiles
{
    public class ServiceInfoProfile : Profile
    {
        public ServiceInfoProfile()
        {
            CreateMap<ServiceInfoForCreatingDto, ServiceInfo>()
                .ForMember(
                    dest => dest.ServiceImageUrls,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ServiceImageUrls))
                ).ForMember(
                   dest => dest.ServiceImageUrls,
                   opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ServiceImageUrls))
                ).ForMember(
                    dest => dest.ProvidServices,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ProvidServices))
                ).ForMember(
                    dest => dest.CenterFacilites,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.CenterFacilites))
                ).ForMember(
                    dest => dest.DiveAssociations,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.DiveAssociations))
                ).ForMember(
                    dest => dest.SupportedLanguages,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.SupportedLanguages))
                ).ForMember(
                    dest => dest.SupportedPayment,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.SupportedPayment))
                ).ForMember(
                    dest => dest.ServiceCenterType,
                    opt => opt.MapFrom(src => (CenterType)Enum.Parse(typeof(CenterType), src.ServiceCenterType))
                );


            CreateMap<ServiceInfo, ServiceInfoForCreatingDto>()
                .ForMember(
                    dest => dest.ServiceImageUrls,
                    opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                   dest => dest.ServiceImageUrls,
                   opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ProvidServices,
                    opt => opt.MapFrom(src => src.ProvidServices.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.CenterFacilites,
                    opt => opt.MapFrom(src => src.CenterFacilites.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.DiveAssociations,
                    opt => opt.MapFrom(src => src.DiveAssociations.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedLanguages,
                    opt => opt.MapFrom(src => src.SupportedLanguages.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedPayment,
                    opt => opt.MapFrom(src => src.SupportedPayment.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ServiceCenterType,
                    opt => opt.MapFrom(src => src.ServiceCenterType.ToString())
                );


            CreateMap<ServiceInfoUpdatingDto, ServiceInfo>()
                .ForMember(
                    dest => dest.ServiceImageUrls,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ServiceImageUrls))
                ).ForMember(
                   dest => dest.ServiceImageUrls,
                   opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ServiceImageUrls))
                ).ForMember(
                    dest => dest.ProvidServices,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.ProvidServices))
                ).ForMember(
                    dest => dest.CenterFacilites,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.CenterFacilites))
                ).ForMember(
                    dest => dest.DiveAssociations,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.DiveAssociations))
                ).ForMember(
                    dest => dest.SupportedLanguages,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.SupportedLanguages))
                ).ForMember(
                    dest => dest.SupportedPayment,
                    opt => opt.MapFrom(src => string.Join(ServiceInfo.urlSplittor, src.SupportedPayment))
                ).ForMember(
                    dest => dest.ServiceCenterType,
                    opt => opt.MapFrom(src => (CenterType)Enum.Parse(typeof(CenterType), src.ServiceCenterType))
                ); ;
            CreateMap<ServiceInfo, ServiceInfoUpdatingDto>()
                .ForMember(
                    dest => dest.ServiceImageUrls,
                    opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                   dest => dest.ServiceImageUrls,
                   opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ProvidServices,
                    opt => opt.MapFrom(src => src.ProvidServices.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.CenterFacilites,
                    opt => opt.MapFrom(src => src.CenterFacilites.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.DiveAssociations,
                    opt => opt.MapFrom(src => src.DiveAssociations.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedLanguages,
                    opt => opt.MapFrom(src => src.SupportedLanguages.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedPayment,
                    opt => opt.MapFrom(src => src.SupportedPayment.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ServiceCenterType,
                    opt => opt.MapFrom(src => src.ServiceCenterType.ToString())
                );

            CreateMap<ServiceInfo, ServiceInfoOutputDto>()
                .ForMember(
                    dest => dest.ServiceImageUrls,
                    opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                   dest => dest.ServiceImageUrls,
                   opt => opt.MapFrom(src => src.ServiceImageUrls.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ProvidServices,
                    opt => opt.MapFrom(src => src.ProvidServices.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.CenterFacilites,
                    opt => opt.MapFrom(src => src.CenterFacilites.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.DiveAssociations,
                    opt => opt.MapFrom(src => src.DiveAssociations.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedLanguages,
                    opt => opt.MapFrom(src => src.SupportedLanguages.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.SupportedPayment,
                    opt => opt.MapFrom(src => src.SupportedPayment.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries))
                ).ForMember(
                    dest => dest.ServiceCenterType,
                    opt => opt.MapFrom(src => src.ServiceCenterType.ToString())
                );


        }
    }
}
