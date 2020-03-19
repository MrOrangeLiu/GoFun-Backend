using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.CoachInfo
{
    public class CoachInfoUpdatingDto
    {
        public string Description { get; set; } // Describe your story, and introduce yourself
        public string Certificates { get; set; }

        public List<string> LocationImageUrls { get; set; } // Images for showing the location where you're instructing
        public List<string> SelfieUrls { get; set; } // Image of yourself
        public string LocationAddress { get; set; } // Instruction Location
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
