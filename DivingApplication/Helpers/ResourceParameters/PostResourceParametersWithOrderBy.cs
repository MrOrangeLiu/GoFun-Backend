using DivingApplication.Entities;
using System;

namespace DivingApplication.Helpers.ResourceParameters
{
    public class PostResourceParametersWithOrderBy : IPostResourceParameters
    {
        // Searching
        public string SearchQuery { get; set; } // Used for searching the title

        public Place Place { get; set; }
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

        public Guid AuthorId { get; set; }
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
                        Fields,
                        OrderBy,
                    };
                case UriType.NextPage:
                    return new
                    {
                        pageNumber = PageNumber + 1,
                        PageSize,
                        searchQuery = SearchQuery,
                        Fields,
                        OrderBy,
                    };
                default:
                    return new
                    {
                        PageNumber,
                        PageSize,
                        SearchQuery,
                        Fields,
                        OrderBy,
                    };
            }

        }
    }
}
