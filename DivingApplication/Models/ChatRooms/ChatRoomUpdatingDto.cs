using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRoom
{
    public class ChatRoomUpdatingDto
    {
        // Only the GroupName can be changed.
        public string GroupName { get; set; }
    }
}
