using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("AuthorId")]
        public User Author { get; set; } // The Person who report this issue
        public Guid AuthorId { get; set; }
        public string ReportContent { get; set; }
        public string ReportReason { get; set; }
        public bool Solved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime SolvedAt { get; set; }
        public string SolvedById { get; set; }
        public string Note { get; set; } // Note for Admin

    }
}
