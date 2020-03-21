using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Messages
{
    public class MessageOutputDto
    {
        [Key]
        public Guid Id { get; set; }
        public UserBriefOutputDto Author { get; set; }
        public Guid AuthorId { get; set; }
        public Guid BelongChatRoomId { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ReadBy { get; set; } = new List<string>();
        public List<string> LikedBy { get; set; } = new List<string>();
    }
}
