using DivingApplication.Entities;
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

        [Required]
        [MinLength(4)]
        [MaxLength(64)]

        public string Title { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }
        public string PostContentType { get; set; }

        [Url]
        public List<string> ContentURL { get; set; } = new List<string>();

        [ForeignKey("AuthorId")]
        public UserBriefOutputDto Author { get; set; }
        public Guid AuthorId { get; set; } // Include Author

        public int PostLikedCount { get; set; }
        public int PostSavedCount { get; set; }

        public int CommentCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
