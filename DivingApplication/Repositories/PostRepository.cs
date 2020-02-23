using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories
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

        public PageList<Post> GetPosts(PostResourceParameters postResourceParameters)
        {
            if (postResourceParameters == null) throw new ArgumentNullException(nameof(postResourceParameters));

            var collection = _context.Posts as IQueryable<Post>;

            if (!string.IsNullOrWhiteSpace(postResourceParameters.SearchQuery))
            {
                var searchinQuery = postResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Title.ToLower().Contains(searchinQuery));
            }

            if (!string.IsNullOrWhiteSpace(postResourceParameters.OrderBy))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutpuDto, Post>();
                collection = collection.ApplySort(postResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            return PageList<Post>.Create(collection, postResourceParameters.PageNumber, postResourceParameters.PageSize);
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
            if (post == null) throw new ArgumentNullException();

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
