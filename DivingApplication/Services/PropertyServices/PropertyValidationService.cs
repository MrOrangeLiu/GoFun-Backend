using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DivingApplication.Services.PropertyServices
{
    public class PropertyValidationService : IPropertyValidationService
    {
        public bool HasValidProperties<T>(string fields) // testing if the fields string contain correct name of fields
        {
            if (string.IsNullOrWhiteSpace(fields)) return true;

            var fieldsAfterSplit = fields.Split(",").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                // Find if the Type T Contain Property Name (Field)
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null) return false;
            }

            return true;
        }
    }
}
