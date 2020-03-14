using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DivingApplication.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public string Content { get; set; }

        [ForeignKey("BelongPostId")]
        public Post BelongPost { get; set; }
        public Guid BelongPostId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }


    }
}
