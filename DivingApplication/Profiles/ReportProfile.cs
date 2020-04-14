using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Profiles
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<ReportForCreatingDto, Report>().ReverseMap();
            CreateMap<Report, ReportOutputDto>();
        }
    }
}
