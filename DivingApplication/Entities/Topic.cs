using DivingApplication.Entities.ManyToManyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Topic
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }
        public List<PostTopic> TopicPosts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
