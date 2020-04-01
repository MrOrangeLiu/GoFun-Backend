using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.ChatRooms
{
    /// <summary>
    /// This one shouldn't have the list field, only for updating the basic ChatRoom info
    /// </summary>
    public class ChatRoomSingalROutputDto
    {
        public Guid Id { get; set; }
        public byte[] GroupProfileImage { get; set; }
        [MaxLength(128)]
        public string GroupName { get; set; }
        [MaxLength(1024)]
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
