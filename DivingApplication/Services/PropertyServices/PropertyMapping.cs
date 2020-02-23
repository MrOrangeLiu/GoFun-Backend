using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Services.PropertyServices
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMappingMarker
    {
        // We Storing the Dictionary here cuz we want the Dictionary can be found by <TSource, TDestination>
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; set; }

        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
