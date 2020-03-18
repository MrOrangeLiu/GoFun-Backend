using DivingApplication.Entities;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.CoachInfo
{
    public class CoachInfoOutputDto
    {
        public Guid Id { get; set; }
        public UserBriefOutputDto Author { get; set; }
        public Guid AuthorId { get; set; }
        public string Description { get; set; } // Describe your story, and introduce yourself
        public List<string> LocationImageUrls { get; set; } // Images for showing the location where you're instructing
        public List<string> SelfieUrls { get; set; } // Image of yourself
        public string LocationAddress { get; set; } // Instruction Location
        public double Lat { get; set; }
        public double Lng { get; set; }
        public Place Place { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
