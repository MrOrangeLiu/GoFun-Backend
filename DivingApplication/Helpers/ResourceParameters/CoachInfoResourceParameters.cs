using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Helpers.ResourceParameters
{
    public class CoachInfoResourceParameters
    {
        // Searching
        public string SearchQuery { get; set; } // Used for searching the title // Seaching for the coach name

        // Pagination
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 20; // Default Page Size

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        // Sorting and Population
        public string OrderBy { get; set; } = "CreatedAt";
        public string Fields { get; set; }

    }
}
