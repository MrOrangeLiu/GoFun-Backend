using DivingApplication.Entities.ManyToManyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class ChatRoom
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public List<UserChatRoom> UserChatRooms { get; set; } = new List<UserChatRoom>();
        public List<Message> Messages { get; set; } = new List<Message>();

    }
}
