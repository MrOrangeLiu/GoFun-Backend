using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Post
    {
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
        public string Description { get; set; }
        public ContentType PostContentType { get; set; }


        public byte[] Content { get; set; } = Array.Empty<byte>();

        [Url]
        public Uri ContentUri { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        public int NumOfLikes { get; set; } = 0;
        public int NumOfSaves { get; set; } = 0;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }



    }
}
