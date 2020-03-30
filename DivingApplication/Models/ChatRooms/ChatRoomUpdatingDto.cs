using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRoom
{
    public class ChatRoomUpdatingDto
    {
        [Required]
        public bool IsGroup { get; set; }

        [MaxLength(128)]
        public string GroupName { get; set; }
        public byte[] GroupProfileImage { get; set; }
        public Place Place { get; set; }
        [MaxLength(1024)]
        public string LocationAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }


        public bool IsValidModel()
        {
            if (string.IsNullOrWhiteSpace(GroupName) || GroupName.Length > 128) return false;
            if (LocationAddress != null || LocationAddress.Length > 1024) return false;
            
            return true;
        }

    }
}
