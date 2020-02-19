using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Message
    {
        public enum MessageContentType
        {
            Text,
            Photo,
        }

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        public MessageContentType MessageType { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        //public List<User> ReadBy { get; set; } = new List<User>();

        //public List<User> LikedBy { get; set; } = new List<User>();


    }
}
