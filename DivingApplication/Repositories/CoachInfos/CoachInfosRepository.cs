using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Services.PropertyServices;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Models.CoachInfo;

namespace DivingApplication.Repositories.CoachInfos
{
    public class CoachInfosRepository : ICoachInfosRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public CoachInfosRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context;
            _propertyMapping = propertyMapping;
        }

        public async Task AddCoachInfo(Guid userId, CoachInfo coachInfo)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            if (coachInfo == null) throw new ArgumentNullException(nameof(coachInfo));

            coachInfo.AuthorId = userId;
            coachInfo.CreatedAt = DateTime.Now;
            coachInfo.UpdateAt = DateTime.Now;


            await _context.CoachInfos.AddAsync(coachInfo);
        }


        public PageList<CoachInfo> GetCoachInfos(CoachInfoResourceParameters coachInfoResourceParameters)
        {
            if (coachInfoResourceParameters == null) throw new ArgumentNullException(nameof(coachInfoResourceParameters));

            var collection = _context.CoachInfos.Include(c =>c.Author) as IQueryable<CoachInfo>;

            if (!string.IsNullOrWhiteSpace(coachInfoResourceParameters.SearchQuery))
            {
                string searchinQuery = coachInfoResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(c => c.Author.Name.ToLower().Contains(searchinQuery) || c.Author.Email.ToLower().Contains(searchinQuery)); // Do we need includes to do this?
            }

            if (!string.IsNullOrWhiteSpace(coachInfoResourceParameters.OrderBy))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<CoachInfoOutputDto, CoachInfo>();
                collection = collection.ApplySort(coachInfoResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<CoachInfo>.Create(collection, coachInfoResourceParameters.PageNumber, coachInfoResourceParameters.PageSize);
        }



        public async Task<CoachInfo> GetCoachInfo(Guid coachId)
        {
            if (coachId == Guid.Empty) throw new ArgumentNullException(nameof(coachId));
            return await _context.CoachInfos.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == coachId).ConfigureAwait(false);
        }

        public async Task<CoachInfo> GetCoachInfoForUser(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
            return await _context.CoachInfos.Include(c => c.Author).FirstOrDefaultAsync(c => c.AuthorId == userId).ConfigureAwait(false);
        }

        public void UpdateCoachInfo(CoachInfo coachInfo)
        {
        }

        public void DeleteCoachInfo(CoachInfo coachInfo)
        {
            if (coachInfo == null) throw new ArgumentNullException(nameof(coachInfo));
            _context.CoachInfos.Remove(coachInfo);
        }


        public async Task<bool> CoachInfoExists(Guid coachId)
        {
            if (coachId == Guid.Empty) throw new ArgumentNullException(nameof(coachId));
            return await _context.CoachInfos.AnyAsync(c => c.Id == coachId).ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync().ConfigureAwait(false)) >= 0);
        }

    }
}
