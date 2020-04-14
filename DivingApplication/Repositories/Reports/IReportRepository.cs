using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Reports
{
    public interface IReportRepository
    {
        Task AddReport(Report report);
        void DeleteReport(Report report);
        Task<Report> GetReport(Guid reportId);
        Task<PageList<Report>> GetReports(ReportResourceParameters resourceParameters);
        Task<bool> Save();
    }
}