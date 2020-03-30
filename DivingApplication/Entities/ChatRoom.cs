using DivingApplication.Entities.ManyToManyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //public List<ChatRoomInviteUser> ChatRoomInviteUsers { get; set; } = new List<ChatRoomInviteUser>();
        //public List<ChatRoomAdminUser> ChatRoomAdminUsers { get; set; } = new List<ChatRoomAdminUser>(); // Admin Must the the memebers
        public List<Message> Messages { get; set; } = new List<Message>();

        //[ForeignKey("OwnerId")]
        //public User Owner { get; set; }
        //public Guid OwnerId { get; set; }

        public byte[] GroupProfileImage { get; set; }
        public bool IsGroup { get; set; }

        [MaxLength(128)]
        public string GroupName { get; set; }

        [MaxLength(1024)]
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
