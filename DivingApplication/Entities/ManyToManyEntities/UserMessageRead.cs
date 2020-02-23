using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities.ManyToManyEntities
{
    public class UserMessageRead
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Message Message { get; set; }
        public Guid MessageId { get; set; }
    }
}
