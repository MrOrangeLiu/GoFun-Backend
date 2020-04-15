using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Reports;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public ReportRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public async Task AddReport(Report report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            await _context.Reports.AddRangeAsync(report);
        }

        public async Task<PageList<Report>> GetReports(ReportResourceParameters resourceParameters)
        {
            if (resourceParameters == null) throw new ArgumentNullException(nameof(resourceParameters));

            var collection = _context.Reports.Include(r => r.Author) as IQueryable<Report>;

            // Checking if we want to find the Reports made by Certain User
            if (resourceParameters.AuthorId != Guid.Empty)
            {
                collection = collection.Where(r => r.AuthorId == resourceParameters.AuthorId);
            }

            // 
            if (resourceParameters.Solved != null)
            {
                collection = collection.Where(r => r.Solved == resourceParameters.Solved);
            }


            if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                collection = collection.Where(r => r.ReportContent.Contains(resourceParameters.SearchQuery));
            }


            if (!string.IsNullOrWhiteSpace(resourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<ReportOutputDto, Report>();
                collection = collection.ApplySort(resourceParameters.OrderBy, postPropertyMappingDictionary);
            }


            return PageList<Report>.Create(collection, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public async Task<Report> GetReport(Guid reportId)
        {
            return await _context.Reports.Include(r => r.Author).SingleOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
        }


        public void DeleteReport(Report report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            _context.Reports.Remove(report);
        }







    }
}
