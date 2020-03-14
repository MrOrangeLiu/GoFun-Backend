using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DivingApplication.Helpers.Extensions
{
    public static class ObjectExtension
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {

            if (source == null) throw new ArgumentNullException(nameof(source));

            var dataShapeObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields)) // Checking if the fields provided
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); // Get info of all the properties 

                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object>)dataShapeObject).Add(propertyInfo.Name, propertyValue);
                }

                return dataShapeObject;
            }


            var fieldsAfterSplit = fields.Split(",").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var filed in fieldsAfterSplit) // Adding the info of the required properties to the list
            {
                var propertyName = filed.Trim();

                // Get the info of the property 
                var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); // Q; What's the Pipe mean?

                // If the info is null, then throw Exception
                if (propertyInfo == null) throw new Exception($"{propertyName.ToString()} was not found");

                // Adding the property to the list

                ((IDictionary<string, object>)dataShapeObject).Add(propertyInfo.Name, propertyInfo.GetValue(source));
            }

            return dataShapeObject;
        }

    }
}
