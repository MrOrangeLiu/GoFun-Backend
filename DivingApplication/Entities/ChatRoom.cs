using DivingApplication.Entities.ManyToManyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class ChatRoom : IHasPlace
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public List<UserChatRoom> UserChatRooms { get; set; } = new List<UserChatRoom>();
        public List<Message> Messages { get; set; } = new List<Message>();
        public byte[] GroupProfileImage { get; set; }
        public bool IsGroup { get; set; }
        public string GroupName { get; set; }
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
