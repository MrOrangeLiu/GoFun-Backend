using DivingApplication.Entities;
using DivingApplication.Models.Messages;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRooms
{
    public class ChatRoomOutputDto
    {
        public Guid Id { get; set; }
        public List<UserBriefOutputDto> Users { get; set; } = new List<UserBriefOutputDto>();
        public List<MessageOutputDto> Messages { get; set; } = new List<MessageOutputDto>();
        public List<Guid> Admins { get; set; } = new List<Guid>(); // Admin Must the the memebers
        public List<Guid> Pendings { get; set; } = new List<Guid>();
        public bool IsGroup { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] GroupProfileImage { get; set; }
        [MaxLength(128)]
        public string GroupName { get; set; }
        [MaxLength(1024)]
        public string LocationAddress { get; set; }
        //public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
