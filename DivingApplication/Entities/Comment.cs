using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public string Content { get; set; }

        [ForeignKey("BelongPostId")]
        public Post BelongPost { get; set; }
        public Guid BelongPostId { get; set; }

        public int NumOfLiks { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


    }
}
