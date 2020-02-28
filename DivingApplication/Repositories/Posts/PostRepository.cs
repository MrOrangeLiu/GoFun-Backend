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

            await _context.Posts.AddRangeAsync(post).ConfigureAwait(false);
        }

        public PageList<Post> GetPosts(PostResourceParameters postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var collection = _context.Posts as IQueryable<Post>;

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                string searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Title.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.OrderBy))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);
        }

        public async Task<PageList<Post>> GetAllFollowingPosts(Guid userId, PostResourceParameters postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var user = await _context.Users.FindAsync(userId);

            List<Guid> allFollowingIds = user.Following.Select(uf => uf.FollowingId).ToList();

            var collection = _context.Posts as IQueryable<Post>;

            collection = collection.Where(p => allFollowingIds.Contains(p.AuthorId));

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                var searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Title.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.OrderBy))
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
            await _context.AddRangeAsync(userPostLike).ConfigureAwait(false);
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
            await _context.AddRangeAsync(userPostSave).ConfigureAwait(false);
            return userPostSave;

        }

        public void UserUnlikeAPost(UserPostSave currentUserPostSave)
        {
            _context.Remove(currentUserPostSave);
        }

        public void UserLikePostExist(Guid userId, Guid postId)
        {

        }


        public async Task<Post> GetPost(Guid postId)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            return await _context.Posts.FindAsync(postId);

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

            return await _context.Posts.AnyAsync(u => u.Id == postId).ConfigureAwait(false);
        }
        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync().ConfigureAwait(false)) >= 0);
        }



    }
}
