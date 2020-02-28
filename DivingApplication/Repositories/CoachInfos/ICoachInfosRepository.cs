using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.CoachInfos
{
    public interface ICoachInfosRepository
    {
        Task AddCoachInfo(Guid userId, CoachInfo coachInfo);
        Task<bool> CoachInfoExists(Guid coachId);
        void DeleteCoachInfo(CoachInfo coachInfo);
        Task<CoachInfo> GetCoachInfo(Guid coachId);
        Task<CoachInfo> GetCoachInfoForUser(Guid userId);
        PageList<CoachInfo> GetCoachInfos(CoachInfoResourceParameters coachInfoResourceParameters);
        Task<bool> Save();
        void UpdateCoachInfo(CoachInfo coachInfo);
    }
}