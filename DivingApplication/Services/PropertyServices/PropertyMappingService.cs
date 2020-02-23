using DivingApplication.Entities;
using DivingApplication.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Services.PropertyServices
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _postPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"Title", new PropertyMappingValue(new List<string>(){"Title"})},
            {"Description", new PropertyMappingValue(new List<string>(){"Description"})},
            {"PostContentType", new PropertyMappingValue(new List<string>(){"PostContentType"})},
            {"ContentURL", new PropertyMappingValue(new List<string>(){"string"})},
            {"Author", new PropertyMappingValue(new List<string>(){"Author"})},
            {"AuthorId", new PropertyMappingValue(new List<string>(){"AuthorId"})},
            {"PostLikedCount", new PropertyMappingValue(new List<string>(){"PostLikedBy"})},
            {"PostSavedCount", new PropertyMappingValue(new List<string>(){"PostSavedBy"})},
            {"CommentCount", new PropertyMappingValue(new List<string>(){"Comments"})},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };

        private IList<IPropertyMappingMarker> _propertyMappings = new List<IPropertyMappingMarker>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<PostOutpuDto, Post>(_postPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {

            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1) return matchingMapping.First().MappingDictionary;

            throw new Exception("No Mapping was found");
        }

        public bool ValidMappingExist<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields)) return true; // No Fields Required

            var fieldsAfterSplit = fields.Split(",");

            foreach (var field in fieldsAfterSplit)
            {
                var trimField = field.Trim();
                var indexOfSpace = trimField.IndexOf(" ");

                var propertyName = indexOfSpace == -1 ? trimField : trimField.Remove(indexOfSpace);

                if (!propertyMapping.ContainsKey(propertyName)) return false;

            }

            return true;
        }





    }
}
