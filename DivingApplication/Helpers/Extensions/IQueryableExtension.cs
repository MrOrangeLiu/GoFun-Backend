using DivingApplication.Services.PropertyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace DivingApplication.Helpers.Extensions
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {

            if (source == null) throw new ArgumentNullException(nameof(source));

            if (mappingDictionary == null) throw new ArgumentNullException(nameof(mappingDictionary));

            if (string.IsNullOrWhiteSpace(orderBy)) return source; // Don't need any sorting here

            var orderBySplit = orderBy.Split(","); // How can we know it should be decending or ascending?

            foreach (var orderByClause in orderBySplit)
            {

                var trimmedOrderBy = orderByClause.Trim();
                var orderDesc = trimmedOrderBy.EndsWith(" desc");
                var indexOfSpace = trimmedOrderBy.IndexOf(" ");
                var propertyName = indexOfSpace == -1 ? trimmedOrderBy : trimmedOrderBy.Remove(indexOfSpace); // the first to indexOfSpace will be kept

                if (!mappingDictionary.ContainsKey(propertyName)) throw new ArgumentException("Mapping doesn't exists for" + propertyName);

                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null) throw new ArgumentNullException(nameof(propertyMappingValue));

                foreach (var destination in propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Revert) // Why do we have to revert again here, for keeping the default value, so the API user don't need to edit it.
                        orderDesc = !orderDesc;

                    // Using the destination or propertyName to do this
                    // Cuz we have to sort the destination
                    var destinationInfo = typeof(T).GetProperty(destination, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyName.Contains("Count"))
                    {
                        if (orderDesc)
                        {
                            source = source.OrderByDescending(t => ((IEnumerable<Guid>)destinationInfo.GetValue(t)).Count());
                        }
                        else
                        {
                            source = source.OrderBy(t => ((IEnumerable<Guid>)destinationInfo.GetValue(t)).Count());
                        }
                    }
                    else
                    {
                        if (orderDesc)
                        {
                            source = source.OrderByDescending(t => destinationInfo.GetValue(t));
                        }
                        else
                        {
                            source = source.OrderBy(t => destinationInfo.GetValue(t));
                        }
                    }

                }

            }

            return source; // We need dynamic linq to make it work

        }
    }
}
