using System;

namespace DivingApplication.Helpers.ResourceParameters
{
    public interface IPostResourceParameters
    {
        Guid AuthorId { get; set; }
        string Fields { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        string SearchQuery { get; set; }
        public object CreateUrlParameters(UriType uriType);
    }
}