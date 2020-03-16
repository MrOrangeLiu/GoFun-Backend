using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Topic;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Topics
{
    public class TopicsRepository : ITopicsRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public TopicsRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public async Task AddTopic(Topic topic)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));

            topic.CreatedAt = DateTime.Now;
            topic.UpdatedAt = DateTime.Now;

            await _context.Topics.AddRangeAsync(topic);
        }



        public PageList<Topic> GetTopics(TopicResourceParameters topicResourceParameters)
        {
            if (topicResourceParameters == null) throw new ArgumentNullException(nameof(topicResourceParameters));

            var collection = _context.Topics as IQueryable<Topic>;

            //_context.Topics.Include(t => t.TopicPosts.Select(t => t.Post));
            //_context.Topics.Include("TopicPosts.Post");



            if (!string.IsNullOrWhiteSpace(topicResourceParameters.SearchQuery))
            {
                string searchinQuery = topicResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Name.ToLower().Contains(searchinQuery));
            }


            if (!string.IsNullOrWhiteSpace(topicResourceParameters.OrderBy) && topicResourceParameters.OrderBy.Contains("hot"))
            {
                var sevenDaysBefore = DateTime.Now.AddDays(-7);
                collection = collection.OrderBy(t => t.TopicPosts.Where(p => p.Post.CreatedAt.CompareTo(sevenDaysBefore) >= 0).Count()); // get all the topics that most added the most in the last one week.
                var removedString = "Hot";
                var indexOfHot = topicResourceParameters.OrderBy.IndexOf(removedString, StringComparison.OrdinalIgnoreCase);
                topicResourceParameters.OrderBy = (indexOfHot < 0) ? topicResourceParameters.OrderBy : topicResourceParameters.OrderBy.Remove(indexOfHot, removedString.Length);
            }

            if (!string.IsNullOrWhiteSpace(topicResourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<TopicOutputDto, Topic>();
                collection = collection.ApplySort(topicResourceParameters.OrderBy, postPropertyMappingDictionary);
            }


            if (!string.IsNullOrWhiteSpace(topicResourceParameters.SearchQuery))
            {
                string searchinQuery = topicResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.OrderBy(t => t.Name.ToLower() == searchinQuery ? 0 : 1);
            }

            return PageList<Topic>.Create(collection, topicResourceParameters.PageNumber, topicResourceParameters.PageSize);
        }


        public async Task<Topic> GetTopic(Guid topicId)
        {
            if (topicId == Guid.Empty) throw new ArgumentNullException(nameof(topicId));
            return await _context.Topics.FindAsync(topicId);
        }


        public async Task UpdateTopic(Topic topic) { }

        public void DeleteTopic(Topic topic)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));

            _context.Topics.Remove(topic);
        }
        public bool TopicExists(Guid topicId)
        {
            if (topicId == Guid.Empty) throw new ArgumentNullException(nameof(topicId));

            return _context.Topics.Any(t => t.Id == topicId);

        }

        public async Task<bool> TopicWithNameExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            return await _context.Topics.AnyAsync(t => t.Name == name);
        }


        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
        }

    }
}
