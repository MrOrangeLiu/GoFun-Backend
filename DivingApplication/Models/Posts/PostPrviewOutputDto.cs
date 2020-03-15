using DivingApplication.Models.Topic;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Posts
{
    public class PostPrviewOutputDto
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }
        public string PostContentType { get; set; }

        public string PreviewURL { get; set; }

        public List<string> ContentURL { get; set; } = new List<string>(); // Changing to Prviews

        [ForeignKey("AuthorId")]
        public UserBriefOutputDto Author { get; set; }
        public Guid AuthorId { get; set; } // Include Author

        public int PostLikedCount { get; set; }
        public string LocationAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public List<TopicOutputDto> PostTopics { get; set; } = new List<TopicOutputDto>();
        public List<UserTaggedOutputDto> TaggedUsers { get; set; } = new List<UserTaggedOutputDto>();
    }
}
