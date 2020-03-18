using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Posts
{
    public class PostUpdatingDto
    {

        [MaxLength(2048)]
        public string Description { get; set; }
        public string LocationAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public Place Place { get; set; }
        public string PreviewURL { get; set; }

        public List<string> ContentURL { get; set; } = new List<string>();

    }
}
