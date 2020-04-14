using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Report;

namespace DivingApplication.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<ReportForCreatingDto, Report>().ForMember(
                    dest => dest.ReportContentType,
                    opt => opt.MapFrom(src => (EReportContentType)Enum.Parse(typeof(EReportContentType), src.ReportContentType))
                );
            CreateMap<Report, ReportOutputDto>().ForMember(
                dest => dest.ReportContentType,
                opt => opt.MapFrom(src => src.ReportContentType.ToString())
                );
        }
    }
}
