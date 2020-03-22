using DivingApplication.Entities;
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
        public byte[] GroupProfileImage { get; set; }

        public List<string> UserIds { get; set; } = new List<string>();

        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
