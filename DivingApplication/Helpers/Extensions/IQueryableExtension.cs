using DivingApplication.Services.PropertyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Linq.Dynamic.Core;
using System.Reflection;
using DivingApplication.Entities;

namespace DivingApplication.Helpers.Extensions
{
    public static class IQueryableExtension
    {




        //public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        //{

        //    if (source == null) throw new ArgumentNullException(nameof(source));

        //    if (mappingDictionary == null) throw new ArgumentNullException(nameof(mappingDictionary));

        //    if (string.IsNullOrWhiteSpace(orderBy)) return source; // Don't need any sorting here

        //    var orderBySplit = orderBy.Split(","); // How can we know it should be decending or ascending?

        //    foreach (var orderByClause in orderBySplit)
        //    {

        //        var trimmedOrderBy = orderByClause.Trim();
        //        var orderDesc = trimmedOrderBy.EndsWith(" desc");
        //        var indexOfSpace = trimmedOrderBy.IndexOf(" ");
        //        var propertyName = indexOfSpace == -1 ? trimmedOrderBy : trimmedOrderBy.Remove(indexOfSpace); // the first to indexOfSpace will be kept

        //        if (!mappingDictionary.ContainsKey(propertyName)) throw new ArgumentException("Mapping doesn't exists for" + propertyName);

        //        var propertyMappingValue = mappingDictionary[propertyName];

        //        if (propertyMappingValue == null) throw new ArgumentNullException(nameof(propertyMappingValue));

        //        foreach (var destination in propertyMappingValue.DestinationProperties.Reverse())
        //        {
        //            if (propertyMappingValue.Revert) // Why do we have to revert again here, for keeping the default value, so the API user don't need to edit it.
        //                orderDesc = !orderDesc;

        //            // Using the destination or propertyName to do this
        //            // Cuz we have to sort the destination
        //            var destinationInfo = typeof(T).GetProperty(destination, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        //            if (propertyName.Contains("Count"))
        //            {
        //                if (orderDesc)
        //                {
        //                    source = source.OrderBy(t => 50); // Desc //((List<Guid>)destinationInfo.GetValue(t)).Count
        //                }

        //                // The Porblem of the DestinationInfo
        //                else
        //                {
        //                    source = source.OrderBy(t => 50);
        //                }
        //            }
        //            else
        //            {
        //                if (orderDesc)
        //                {
        //                    source = source.OrderBy(t => 50);// Desc // destinationInfo.GetValue(t)
        //                }
        //                else
        //                {
        //                    source = source.OrderBy(t => 50);
        //                }
        //            }

        //        }

        //    }

        //    return source; // We need dynamic linq to make it work

        //}

        static public IQueryable<Post> searchByTaggedUser(this IQueryable<Post> collection, Guid taggedUserId)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (taggedUserId == null) throw new ArgumentNullException(nameof(taggedUserId));

            return collection.Where(p => p.TaggedUsers.Select(t => t.UserId).Contains(taggedUserId));
        }


        static public IQueryable<IHasPlace> SearchingByPlace(this IQueryable<IHasPlace> collection, Place place)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (place == null) throw new ArgumentNullException(nameof(place));

            if (!string.IsNullOrWhiteSpace(place.AdministrativeArea))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.Country))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.CountryCode))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.Locality))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.Name))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.PostalCode))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.SubAdministrativeArea))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.SubLocality))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.SubThoroughfare))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            if (!string.IsNullOrWhiteSpace(place.Thoroughfare))
            {
                collection = collection.Where(p => p.Place.AdministrativeArea == place.AdministrativeArea);
            }
            return collection;
        }

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            var orderByString = "";

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (mappingDictionary == null)
                throw new ArgumentNullException(nameof(mappingDictionary));


            if (string.IsNullOrWhiteSpace(orderBy))
                return source; // Don't need any sorting here


            var orderBySplit = orderBy.Split(",").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)); // How can we know it should be decending or ascending?

            foreach (var orderByClause in orderBySplit)
            {

                var trimmedOrderBy = orderByClause.Trim();
                var orderDesc = trimmedOrderBy.EndsWith(" desc");
                var indexOfSpace = trimmedOrderBy.IndexOf(" ");
                var propertyName = indexOfSpace == -1 ? trimmedOrderBy : trimmedOrderBy.Remove(indexOfSpace); // the first to indexOfSpace will be kept

                //if (propertyName.ToLower() == "Hot") continue; // Ignoreing the Hot Ordering here


                if (!mappingDictionary.ContainsKey(propertyName))
                    throw new ArgumentException("Mapping doesn't exists for" + propertyName);

                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                    throw new ArgumentNullException(nameof(propertyMappingValue));


                foreach (var destination in propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Revert) // Why do we have to revert again here, for keeping the default value, so the API user don't need to edit it.
                        orderDesc = !orderDesc;

                    orderByString = orderByString + (!string.IsNullOrWhiteSpace(orderByString) ? "," : "") + destination + (orderDesc ? " descending" : " ascending");
                }
                // + (propertyName.Contains("Count") ? ".Count" : "")
            }

            return source.OrderBy(orderByString); // We need dynamic linq to make it work

        }
    }
}
