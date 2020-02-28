using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.ServiceInfos
{
    public interface IServiceInfosRepository
    {
        Task AddServiceInfo(ServiceInfo serviceInfo, Guid ownerId);
        void DeletePost(ServiceInfo serviceInfo);
        Task<ServiceInfo> GetServiceInfo(Guid serviceInfoId);
        PageList<ServiceInfo> GetServiceInfos(ServiceInfoResourceParameters serviceInfoResourceParameters);
        Task<bool> Save();
        Task<bool> ServiceExists(Guid serviceInfoId);
        Task UpdateServiceInfo(ServiceInfo serviceInfo);
    }
}