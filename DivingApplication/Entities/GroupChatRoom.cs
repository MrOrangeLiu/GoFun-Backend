using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class GroupChatRoom : ChatRoom
    {
        public string GroupName { get; set; }
        public byte[] GroupPhoto { get; set; }
    }
}
