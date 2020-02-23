using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Services.PropertyServices
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; set; } // Store the Destination Property name (From Entity not Dto)

        public bool Revert { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties;
            Revert = revert;
        }
    }
}
