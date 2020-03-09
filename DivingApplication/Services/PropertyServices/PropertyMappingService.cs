using DivingApplication.Entities;
using DivingApplication.Models.CoachInfo;
using DivingApplication.Models.Comments;
using DivingApplication.Models.Posts;
using DivingApplication.Models.ServiceInfo;
using DivingApplication.Models.Topic;
using DivingApplication.Models.Users;
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
            {"ContentURL", new PropertyMappingValue(new List<string>(){"ContentURL"})},
            {"Author", new PropertyMappingValue(new List<string>(){"Author"})},
            {"AuthorId", new PropertyMappingValue(new List<string>(){"AuthorId"})},
            {"PostLikedCount", new PropertyMappingValue(new List<string>(){"PostLikedBy.Count"}, true)},
            {"PostSavedCount", new PropertyMappingValue(new List<string>(){"PostSavedBy.Count"}, true)},
            {"CommentCount", new PropertyMappingValue(new List<string>(){"Comments.Count"}, true)},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
            {"Views", new PropertyMappingValue(new List<string>(){"Views"}, true)},
            {"LocationAddress", new PropertyMappingValue(new List<string>(){"LocationAddress"}, true)},
            {"LatLng", new PropertyMappingValue(new List<string>(){"LatLng"}, true)},

        };


        private Dictionary<string, PropertyMappingValue> _commentPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"Content", new PropertyMappingValue(new List<string>(){"Content"})},
            {"BelongPostId", new PropertyMappingValue(new List<string>(){"BelongPostId"})},
            {"AuthorId", new PropertyMappingValue(new List<string>(){"AuthorId"})},
            {"Author", new PropertyMappingValue(new List<string>(){"Author"})},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };



        private Dictionary<string, PropertyMappingValue> _coachInfoPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"AuthorId", new PropertyMappingValue(new List<string>(){"AuthorId"})},
            {"Author", new PropertyMappingValue(new List<string>(){"Author"})},
            {"Author.Followers.Count", new PropertyMappingValue(new List<string>(){"Author.Followers.Count"},true)},
            {"Description", new PropertyMappingValue(new List<string>(){"Description"})},
            {"LocationImageUrls", new PropertyMappingValue(new List<string>(){"LocationImageUrls"})},
            {"SelfieUrls", new PropertyMappingValue(new List<string>(){"SelfieUrls"})},
            {"InsturctingLocation", new PropertyMappingValue(new List<string>(){"InsturctingLocation"})},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };

        private Dictionary<string, PropertyMappingValue> _serviceInfoPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"OwnerId", new PropertyMappingValue(new List<string>(){"OwnerId"})},
            {"Owner", new PropertyMappingValue(new List<string>(){"Owner"})},
            {"Owner.Followers.Count", new PropertyMappingValue(new List<string>(){"Owner.Followers.Count"},true)},
            {"ServiceImageUrls", new PropertyMappingValue(new List<string>(){"ServiceImageUrls"})},
            {"CenterName", new PropertyMappingValue(new List<string>(){"CenterName"})},
            {"LocalCenterName", new PropertyMappingValue(new List<string>(){"LocalCenterName"})},
            {"PhoneNumber", new PropertyMappingValue(new List<string>(){"PhoneNumber"})},
            {"Email", new PropertyMappingValue(new List<string>(){"Email"})},
            {"Website", new PropertyMappingValue(new List<string>(){"Website"})},
            {"SocailMedia", new PropertyMappingValue(new List<string>(){"SocailMedia"})},
            {"SocialMediaAccount", new PropertyMappingValue(new List<string>(){"SocialMediaAccount"})},
            {"CenterOpeningDate", new PropertyMappingValue(new List<string>(){"CenterOpeningDate"})},
            {"DetailedAddress", new PropertyMappingValue(new List<string>(){"DetailedAddress"})},
            {"CenterDescription", new PropertyMappingValue(new List<string>(){"CenterDescription"})},
            {"ProvideAccommodation", new PropertyMappingValue(new List<string>(){"ProvideAccommodation"})},
            {"ProvidServices", new PropertyMappingValue(new List<string>(){"ProvidServices"})},
            {"CenterFacilites", new PropertyMappingValue(new List<string>(){"CenterFacilites"})},
            {"DiveAssociations", new PropertyMappingValue(new List<string>(){"DiveAssociations"})},
            {"SupportedLanguages", new PropertyMappingValue(new List<string>(){"SupportedLanguages"})},
            {"SupportedPayment", new PropertyMappingValue(new List<string>(){"SupportedPayment"})},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };


        private Dictionary<string, PropertyMappingValue> _topicPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"Name", new PropertyMappingValue(new List<string>(){"Name"})},
            {"TopicPostsCount", new PropertyMappingValue(new List<string>(){"TopicPosts.Count"},true)},
            {"Hot", new PropertyMappingValue(new List<string>(){"Hot"})},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };



        private Dictionary<string, PropertyMappingValue> _userPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"})},
            {"Email", new PropertyMappingValue(new List<string>(){"Id"})},
            {"Name", new PropertyMappingValue(new List<string>(){"Name"})},
            {"ProfileImage", new PropertyMappingValue(new List<string>(){"ProfileImage"})},
            {"Familiarity", new PropertyMappingValue(new List<string>(){"Familiarity"})},
            {"UserRole", new PropertyMappingValue(new List<string>(){"UserRole"})},
            {"UserGender", new PropertyMappingValue(new List<string>(){"UserGender"})},

            {"LastSeen", new PropertyMappingValue(new List<string>(){"LastSeen"})},
            {"FollowersCount", new PropertyMappingValue(new List<string>(){"Followers.Count"}, true)},
            {"FollowingCount", new PropertyMappingValue(new List<string>(){"Following.Count"}, true)},
            {"OwningPostsCount", new PropertyMappingValue(new List<string>(){"OwningPosts.Count"}, true)},
            {"OwningServiceInfosCount", new PropertyMappingValue(new List<string>(){"OwningServiceInfos.Count"}, true)},
            {"CreatedAt", new PropertyMappingValue(new List<string>(){"CreatedAt"}, true)},
            {"UpdatedAt", new PropertyMappingValue(new List<string>(){"UpdatedAt"}, true)},
        };

        private IList<IPropertyMappingMarker> _propertyMappings = new List<IPropertyMappingMarker>();


        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<PostOutputDto, Post>(_postPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CommentOutputDto, Comment>(_commentPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CoachInfoOutputDto, CoachInfo>(_coachInfoPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<ServiceInfoOutputDto, ServiceInfo>(_serviceInfoPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<TopicOutputDto, Topic>(_topicPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<UserBriefOutputDto, User>(_userPropertyMapping));
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
