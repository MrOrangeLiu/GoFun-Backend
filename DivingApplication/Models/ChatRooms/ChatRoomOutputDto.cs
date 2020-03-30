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

        public List<UserBriefOutputDto> Users { get; set; } = new List<UserBriefOutputDto>();
        public List<MessageOutputDto> Messages { get; set; } = new List<MessageOutputDto>();
        public List<Guid> Admins { get; set; } = new List<Guid>(); // Admin Must the the memebers
        public List<Guid> Pendings { get; set; } = new List<Guid>();
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] GroupProfileImage { get; set; }
        #region GroupNeededFields
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        #endregion

    }
}
