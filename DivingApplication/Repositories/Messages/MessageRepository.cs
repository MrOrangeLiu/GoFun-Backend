using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Messages;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Messages
{
    public class MessageRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public MessageRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public async Task AddMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.CreatedAt = DateTime.Now;

            await _context.Messages.AddRangeAsync(message);
        }


        public async Task<Message> GetMessage(Guid messageId)
        {
            if (messageId == Guid.Empty) throw new ArgumentNullException(nameof(messageId));

            return await _context.Messages.Include(m => m.Author).SingleOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<List<Message>> GetMessages(MessageResourceParameters resourceParameters)
        {
            if (resourceParameters == null) throw new ArgumentNullException(nameof(resourceParameters));


            var collection = _context.Messages.Include(m => m.Author) as IQueryable<Message>;

            if (resourceParameters.RoomId != Guid.Empty)
            {
                collection = collection.Where(m => m.BelongChatRoomId == resourceParameters.RoomId);
            }

            if (resourceParameters.AfterMessageId != null)
            {
                var afterMessage = await _context.Messages.FindAsync(resourceParameters.AfterMessageId);

                if (afterMessage == null) throw new Exception("Cannot find afterMessage");

                collection = collection.Where(m => m.CreatedAt.CompareTo(afterMessage.CreatedAt) >= 0 && m.Id != resourceParameters.AfterMessageId);
            }

            if (resourceParameters.BeforeMessageId != null)
            {
                var beforeMessage = await _context.Messages.FindAsync(resourceParameters.BeforeMessageId);

                if (beforeMessage == null) throw new Exception("Cannot find beforeMessage");

                collection = collection.Where(m => m.CreatedAt.CompareTo(beforeMessage.CreatedAt) <= 0 && m.Id != resourceParameters.BeforeMessageId);
            }

            if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                collection = collection.Where(m => m.Content.Contains(resourceParameters.SearchQuery));
            }


            if (!string.IsNullOrWhiteSpace(resourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<MessageOutputDto, Message>();
                collection = collection.ApplySort(resourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<Message>.Create(collection, resourceParameters.PageNumber, resourceParameters.PageSize);

        }

        public void DeleteMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            _context.Messages.Remove(message);
        }

    }
}
