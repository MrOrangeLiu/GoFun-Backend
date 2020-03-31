using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities.ManyToManyEntities
{
    public class UserChatRoom
    {
        public enum UserChatRoomRole
        {
            Pending,
            Normal,
            Admin,
            Owner
        }

        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }

        // public
        public UserChatRoomRole Role { get; set; }
    }
}
