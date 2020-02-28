using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.ServiceInfo;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.ServiceInfos
{
    public class ServiceInfosRepository : IServiceInfosRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public ServiceInfosRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public async Task AddServiceInfo(ServiceInfo serviceInfo, Guid ownerId)
        {
            if (serviceInfo == null) throw new ArgumentNullException(nameof(serviceInfo));
            if (ownerId == Guid.Empty) throw new ArgumentNullException(nameof(ownerId));

            serviceInfo.OwnerId = ownerId;
            serviceInfo.CreatedAt = DateTime.Now;
            serviceInfo.UpdatedAt = DateTime.Now;

            await _context.ServiceInfos.AddRangeAsync(serviceInfo).ConfigureAwait(false);
        }

        public PageList<ServiceInfo> GetServiceInfos(ServiceInfoResourceParameters serviceInfoResourceParameters)
        {
            if (serviceInfoResourceParameters == null) throw new ArgumentNullException(nameof(serviceInfoResourceParameters));

            var collection = _context.ServiceInfos as IQueryable<ServiceInfo>;

            if (!string.IsNullOrWhiteSpace(serviceInfoResourceParameters.SearchQuery))
            {
                string searchinQuery = serviceInfoResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(s => s.CenterName.ToLower().Contains(searchinQuery) || s.LocalCenterName.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(serviceInfoResourceParameters.OrderBy))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<ServiceInfoOutputDto, ServiceInfo>();
                collection = collection.ApplySort(serviceInfoResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<ServiceInfo>.Create(collection, serviceInfoResourceParameters.PageNumber, serviceInfoResourceParameters.PageSize);
        }

        public async Task<ServiceInfo> GetServiceInfo(Guid serviceInfoId)
        {
            if (serviceInfoId == Guid.Empty) throw new ArgumentNullException(nameof(serviceInfoId));
            return await _context.ServiceInfos.FindAsync(serviceInfoId).ConfigureAwait(false);
        }

        public async Task UpdateServiceInfo(ServiceInfo serviceInfo)
        {
        }

        public void DeletePost(ServiceInfo serviceInfo)
        {
            if (serviceInfo == null) throw new ArgumentNullException(nameof(serviceInfo));
            _context.ServiceInfos.Remove(serviceInfo);
        }

        public async Task<bool> ServiceExists(Guid serviceInfoId)
        {
            if (serviceInfoId == Guid.Empty) throw new ArgumentNullException(nameof(serviceInfoId));
            return await _context.ServiceInfos.AnyAsync(u => u.Id == serviceInfoId).ConfigureAwait(false);
        }
        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync().ConfigureAwait(false)) >= 0);
        }
    }
}
