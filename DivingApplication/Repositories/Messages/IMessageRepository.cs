using DivingApplication.Entities;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Messages
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(Guid messageId);
        Task<List<Message>> GetMessages(MessageResourceParameters resourceParameters);
        Task<bool> Save();
    }
}