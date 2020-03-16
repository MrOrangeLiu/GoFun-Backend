using DivingApplication.Entities;
using DivingApplication.Models.Comments;
using DivingApplication.Models.Topic;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Post;

namespace DivingApplication.Models.Posts
{
    public class PostOutputDto
    {
        [Key]
        public Guid Id { get; set; }


        [MaxLength(2048)]
        public string Description { get; set; }
        public string PostContentType { get; set; }

        public string PreviewURL { get; set; }

        public List<string> ContentURL { get; set; } = new List<string>();

        [ForeignKey("AuthorId")]
        public UserBriefOutputDto Author { get; set; }
        public Guid AuthorId { get; set; } // Include Author

        public int PostLikedCount { get; set; }
        public int PostSavedCount { get; set; }
        public int Views { get; set; }

        public List<CommentOutputDto> Comments { get; set; } = new List<CommentOutputDto>();

        public int CommentCount { get; set; }
        public string LocationAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<TopicOutputDto> PostTopics { get; set; } = new List<TopicOutputDto>();
        public List<UserBriefOutputDto> TaggedUsers { get; set; } = new List<UserBriefOutputDto>();
    }
}
