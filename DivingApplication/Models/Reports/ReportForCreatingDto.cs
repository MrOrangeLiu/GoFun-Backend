using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Reports
{
    public class ReportForCreatingDto
    {
        public string ReportContentType { get; set; }
        public string ReportContent { get; set; }
        public string ReportReason { get; set; }
    }
}
