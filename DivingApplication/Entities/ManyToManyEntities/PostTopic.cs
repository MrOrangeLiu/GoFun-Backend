using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities.ManyToManyEntities
{
    public class PostTopic
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public Guid TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
