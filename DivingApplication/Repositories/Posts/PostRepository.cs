using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Posts
{
    public class PostRepository : IPostRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public PostRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public async Task AddPost(Post post, Guid authorId)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            if (authorId == Guid.Empty) throw new ArgumentNullException(nameof(authorId));

            post.AuthorId = authorId;
            post.CreatedAt = DateTime.Now;
            post.UpdatedAt = DateTime.Now;

            await _context.Posts.AddRangeAsync(post);
        }

        public PageList<Post> GetPosts(PostResourceParametersWithOrderBy postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var collection = _context.Posts
                .Include(p => p.Author)
                .ThenInclude(a => a.CoachInfo)
                .Include(p => p.Comments)
                .Include(p => p.PostLikedBy)
                .Include(p => p.PostSavedBy)
                .Include(p => p.PostTopics)
                .ThenInclude(t => t.Topic)
                .Include(p => p.TaggedUsers)
                .ThenInclude(u => u.User) as IQueryable<Post>;

            if (postResourceParameters.AuthorId != Guid.Empty)
            {
                collection.Where(p => p.AuthorId == postResourceParameters.AuthorId);
            }


            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                string searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Description.ToLower().Contains(searchinQuery));
            }


            if (!string.IsNullOrWhiteSpace(postResourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);
        }

        public PageList<Post> GetNearbyPosts(PostResourceParametersForNearby postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var collection = _context.Posts
                .Include(p => p.Author)
                .ThenInclude(a => a.CoachInfo)
                .Include(p => p.Comments)
                .Include(p => p.PostLikedBy)
                .Include(p => p.PostSavedBy)
                .Include(p => p.PostTopics)
                .ThenInclude(t => t.Topic)
                .Include(p => p.TaggedUsers)
                .ThenInclude(u => u.User) as IQueryable<Post>;

            if (postResourceParameters.AuthorId != Guid.Empty)
            {
                collection.Where(p => p.AuthorId == postResourceParameters.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                string searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Description.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            double multiplyToRadians = (Math.PI / 180);

            double lat1 = postResourceParameters.Lat;
            double lng1 = postResourceParameters.Lng;


            collection = collection.OrderBy((p) => Math.Atan2(Math.Sqrt(Math.Sin((p.Lat - lat1) * multiplyToRadians / 2) * Math.Sin((p.Lat - lat1) * multiplyToRadians / 2) + Math.Cos(lat1 * multiplyToRadians) * Math.Cos(p.Lat * multiplyToRadians) + Math.Sin((p.Lng - lng1) * multiplyToRadians / 2) * Math.Sin((p.Lng - lng1) * multiplyToRadians / 2)), Math.Sqrt(1 - Math.Sin((p.Lat - lat1) * multiplyToRadians / 2) * Math.Sin((p.Lat - lat1) * multiplyToRadians / 2) + Math.Cos(lat1 * multiplyToRadians) * Math.Cos(p.Lat * multiplyToRadians) + Math.Sin((p.Lng - lng1) * multiplyToRadians / 2) * Math.Sin((p.Lng - lng1) * multiplyToRadians / 2)))
            );

            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);


        }




        public PageList<Post> GetHotPosts(PostResourceParametersForHot postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var collection = _context.Posts
                .Include(p => p.Author)
                .ThenInclude(a => a.CoachInfo)
                .Include(p => p.Comments)
                .Include(p => p.PostLikedBy)
                .Include(p => p.PostSavedBy)
                .Include(p => p.PostTopics)
                .ThenInclude(t => t.Topic)
                .Include(p => p.TaggedUsers)
                .ThenInclude(u => u.User) as IQueryable<Post>;

            if (postResourceParameters.AuthorId != Guid.Empty)
            {
                collection.Where(p => p.AuthorId == postResourceParameters.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                string searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Description.ToLower().Contains(searchinQuery));
            }


            if (!string.IsNullOrWhiteSpace(postResourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }


            var currentTime = DateTime.Now;


            int currentTimeHour = (currentTime.Year * 8760) + (currentTime.Month * 720) + (currentTime.Day * 24) + currentTime.Hour;


            collection = collection.OrderByDescending(p =>
           ((p.Views * 1) + (p.PostLikedBy.Count * 5) + (p.PostSavedBy.Count * 25) + (p.Comments.Count * 30)) / Math.Pow(2.718281828,
           (double)(currentTimeHour - ((p.CreatedAt.Year * 8760) + (p.CreatedAt.Month * 720) + (p.CreatedAt.Day * 24) + p.CreatedAt.Hour)))
          );




            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);


        }

        public async Task<PageList<Post>> GetAllFollowingPosts(Guid userId, PostResourceParametersWithOrderBy postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var user = await _context.Users.FindAsync(userId);

            List<Guid> allFollowingIds = user.Following.Select(uf => uf.FollowingId).ToList();

            var collection = _context.Posts.Include(p => p.Author)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .ThenInclude(a => a.CoachInfo)
                .Include(p => p.PostLikedBy)
                .Include(p => p.PostSavedBy)
                .Include(p => p.PostTopics)
                .ThenInclude(t => t.Topic)
                .Include(p => p.TaggedUsers)
                .ThenInclude(u => u.User) as IQueryable<Post>;

            collection = collection.Where(p => allFollowingIds.Contains(p.AuthorId));

            if (postResourceParameters.AuthorId != Guid.Empty)
            {
                collection.Where(p => p.AuthorId == postResourceParameters.AuthorId);
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                var searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Description.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);
        }


        public async Task<UserPostLike> GetCurrentUserPostLike(Guid userId, Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            return (UserPostLike)await _context.FindAsync(typeof(UserPostLike), userId, postId);
        }

        public async Task<UserPostLike> UserLikePost(Guid userId, Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            var userPostLike = new UserPostLike
            {
                PostId = postId,
                UserId = userId,
            };
            await _context.AddRangeAsync(userPostLike);
            return userPostLike;

        }

        public void UserUnlikeAPost(UserPostLike currentUserPostLike)
        {
            _context.Remove(currentUserPostLike);
        }


        public async Task<UserPostSave> GetCurrentUserPostSave(Guid userId, Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            return (UserPostSave)await _context.FindAsync(typeof(UserPostSave), userId, postId);
        }

        public async Task<UserPostSave> UserSavePost(Guid userId, Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            var userPostSave = new UserPostSave
            {
                PostId = postId,
                UserId = userId,
            };
            await _context.AddRangeAsync(userPostSave);
            return userPostSave;

        }

        public void UserUnlikeAPost(UserPostSave currentUserPostSave)
        {
            _context.Remove(currentUserPostSave);
        }




        public async Task<Post> GetPost(Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            return await _context.Posts.
                Include(p => p.Author)
                .Include(p => p.Comments)
                .Include(p => p.PostLikedBy)
                .Include(p => p.PostSavedBy)
                .Include(p => p.PostTopics)
                .ThenInclude(t => t.Topic)
                .Include(p => p.TaggedUsers)
                .ThenInclude(u => u.User).SingleOrDefaultAsync(p => p.Id == postId);

        }

        public async Task UpdatePost(Post post)
        {
        }

        public void DeletePost(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));

            _context.Posts.Remove(post);
        }

        public async Task<bool> PostExists(Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));

            return await _context.Posts.AnyAsync(u => u.Id == postId);
        }
        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
        }



    }
}
