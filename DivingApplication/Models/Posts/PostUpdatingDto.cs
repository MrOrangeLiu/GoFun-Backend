using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Posts
{
    public class PostUpdatingDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(64)]
        public string Title { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }
        public string LocationAddress { get; set; }
        public string LatLng { get; set; }
        public List<string> ContentURL { get; set; } = new List<string>();

    }
}
