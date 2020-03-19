using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Helpers.ResourceParameters
{
    public class PostResourceParametersForNearby : IPostResourceParameters
    {
        // Searching
        public string SearchQuery { get; set; } // Used for searching the title
        public Guid TaggedTopicId { get; set; }
        public Guid TaggedUserId { get; set; }
        public Guid SavedUserId { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public string OrderBy { get; set; }
        public Place Place { get; set; }
        // Pagination
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 20; // Default Page Size

        public Guid AuthorId { get; set; }


        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;

        }

        // Sorting and Population
        public string Fields { get; set; }


        public object CreateUrlParameters(UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:

                    return new
                    {
                        pageNumber = PageNumber - 1,
                        PageSize,
                        SearchQuery,
                        Fields
                    };
                case UriType.NextPage:
                    return new
                    {
                        pageNumber = PageNumber + 1,
                        PageSize,
                        searchQuery = SearchQuery,
                        Fields
                    };
                default:
                    return new
                    {
                        PageNumber,
                        PageSize,
                        SearchQuery,
                        Fields
                    };
            }

        }
    }
}
