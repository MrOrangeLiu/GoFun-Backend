using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DivingApplication.Models.CoachInfo
{
    public class CoachInfoForCreatingDto
    {
        public Guid Id { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; } // Describe your story, and introduce yourself
        public List<string> LocationImageUrls { get; set; } // Images for showing the location where you're instructing
        public List<string> SelfieUrls { get; set; } // Image of yourself
        public string InsturctingLocation { get; set; } // Instruction Location
    }
}
