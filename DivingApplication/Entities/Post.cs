using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    [StringContentURLAmountLimit]
    public class Post
    {

        public static readonly string urlSplittor = "{%}";

        public enum ContentType
        {
            Image,
            Vedio,
        }


        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(64)]

        public string Title { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }
        public ContentType PostContentType { get; set; }

        [Url]
        public string ContentURL { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        public List<UserPostLike> PostLikedBy { get; set; } = new List<UserPostLike>();
        public List<UserPostSave> PostSavedBy { get; set; } = new List<UserPostSave>();

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<PostTopic> PostTopics { get; set; } = new List<PostTopic>();
        public List<UserPostTag> TaggedUsers { get; set; } = new List<UserPostTag>();

        public int Views { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string LocationAddress { get; set; }
        public string LatLng { get; set; }


    }
}
