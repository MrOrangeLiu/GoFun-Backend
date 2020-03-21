using DivingApplication.Entities;
using DivingApplication.Models.Messages;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRoom
{
    public class ChatRoomOutputDto
    {
        public Guid Id { get; set; }
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public List<UserOutputDto> Users { get; set; } = new List<UserOutputDto>();
        public List<MessageOutputDto> Messages { get; set; } = new List<MessageOutputDto>();
    }
}
