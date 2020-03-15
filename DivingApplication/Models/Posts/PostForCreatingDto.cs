using DivingApplication.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Posts
{
    [ContentURLAmountLimit(ErrorMessage = "The ContentType or PostContentURL is not valid")]
    public class PostForCreatingDto
    {

        [MaxLength(2048)]
        public string Description { get; set; }
        [Required]
        public string PostContentType { get; set; }
        public string LocationAddress { get; set; }
        public double Lat { get; set; }

        public double Lng { get; set; }

        public string PreviewURL { get; set; }

        [Required]
        public List<string> ContentURL { get; set; } = new List<string>();
        public List<string> PostTopicsIds { get; set; } = new List<string>();  // TaggedTopics Id 
        public List<string> TaggedUsersIds { get; set; } = new List<string>(); // TaggedUsers Id

    }
}
