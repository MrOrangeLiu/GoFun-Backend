using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRoom
{
    public class ChatRoomForCreatingDto
    {
        [Required]
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public List<string> UserIds { get; set; } = new List<string>();
    }
}
