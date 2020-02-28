using DivingApplication.Entities;
using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Comments
{
    public class CommentOutputDto
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid BelongPostId { get; set; }

        public Guid AuthorId { get; set; }

        public UserBriefOutputDto Author { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
