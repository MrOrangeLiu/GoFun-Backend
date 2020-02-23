using System.Collections.Generic;

namespace DivingApplication.Services.PropertyServices
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExist<TSource, TDestination>(string fields);
    }
}