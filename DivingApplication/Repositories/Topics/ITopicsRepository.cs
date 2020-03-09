using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Topics
{
    public interface ITopicsRepository
    {
        Task AddTopic(Topic topic);
        void DeleteTopic(Topic topic);
        Task<Topic> GetTopic(Guid topicId);
        PageList<Topic> GetTopics(TopicResourceParameters topicResourceParameters);
        Task<bool> Save();
        bool TopicExists(Guid topicId);
        Task<bool> TopicWithNameExists(string name);
        Task UpdateTopic(Topic topic);
    }
}