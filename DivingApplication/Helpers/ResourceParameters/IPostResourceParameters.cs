using DivingApplication.Entities;
using System;

namespace DivingApplication.Helpers.ResourceParameters
{
    public interface IPostResourceParameters
    {
        Place Place { get; set; }
        Guid AuthorId { get; set; }
        Guid TaggedTopicId { get; set; }
        Guid TaggedUserId { get; set; }
        Guid SavedUserId { get; set; }
        string Fields { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        string SearchQuery { get; set; }
        public object CreateUrlParameters(UriType uriType);
    }
}