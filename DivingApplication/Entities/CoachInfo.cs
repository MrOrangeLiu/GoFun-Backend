using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class CoachInfo : IHasPlace
    {

        public static readonly string urlSplittor = "{%}";

        [Key]
        public Guid Id { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        public Guid AuthorId { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; } // Describe your story, and introduce yourself
        public string LocationImageUrls { get; set; } // Images for showing the location where you're instructing
        public string SelfieUrls { get; set; } // Image of yourself

        [MaxLength(2048)]
        public string LocationAddress { get; set; }
        public Place Place { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }


        public string InsturctingLocation { get; set; } // Instruction Location
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }


    }

}
