using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Topic
{
    public class TopicForCreatingDto
    {
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
