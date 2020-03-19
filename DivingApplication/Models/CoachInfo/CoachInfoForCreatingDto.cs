using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DivingApplication.Models.CoachInfo
{
    public class CoachInfoForCreatingDto
    {
        [MaxLength(2048)]
        public string Description { get; set; } // Describe your story, and introduce yourself

        public string Certificates { get; set; }
        public List<string> LocationImageUrls { get; set; } // Images for showing the location where you're instructing
        public List<string> SelfieUrls { get; set; } // Image of yourself
        public string LocationAddress { get; set; } // Instruction Location, TODO: Add Validation
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }


    }
}
