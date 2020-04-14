using DivingApplication.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Reports
{
    public class ReportOutputDto
    {
        public Guid Id { get; set; }
        public UserBriefOutputDto Author { get; set; } // The Person who report this issue
        public Guid AuthorId { get; set; }
        public string ReportContent { get; set; }
        public string ReportReason { get; set; }
        public bool Solved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime SolvedAt { get; set; }
        public string ReportContentType { get; set; }
        public string SolvedById { get; set; }
        public string Note { get; set; }

    }
}
