using DivingApplication.DbContexts;
using DivingApplication.Services.PropertyServices;
using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DivingApplication.Repositories.Comments
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public CommentRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context;
            _propertyMapping = propertyMapping;
        }

        public async Task AddComment(Comment comment, Guid postId, Guid authorId)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));
            if (authorId == Guid.Empty) throw new ArgumentNullException(nameof(authorId));


            comment.AuthorId = authorId;
            comment.BelongPostId = postId;
            comment.CreatedAt = DateTime.Now;
            comment.UpdatedAt = DateTime.Now;

            await _context.Comments.AddRangeAsync(comment).ConfigureAwait(false);
        }

        public async Task<Comment> GetComment(Guid commentId)
        {
            if (commentId == Guid.Empty) throw new ArgumentNullException(nameof(commentId));

            return await _context.Comments.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public PageList<Comment> GetCommentsForPost(Guid postId, CommentResourceParameters commentResourceParameters)
        {
            if (postId == Guid.Empty) throw new ArgumentNullException(nameof(postId));


            var collection = _context.Comments as IQueryable<Comment>;

            // No SearchingQuery Needed

            collection = collection.Include(c => c.Author).Where(c => c.BelongPostId == postId);

            if (!string.IsNullOrWhiteSpace(commentResourceParameters.OrderBy))
            {
                var commentPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<PostOutputDto, Post>();
                collection = collection.ApplySort(commentResourceParameters.OrderBy, commentPropertyMappingDictionary);
            }

            return PageList<Comment>.Create(collection, commentResourceParameters.PageNumber, commentResourceParameters.PageSize);
        }

        public void UpdateComment(Comment comment) { }

        public void DeleteComment(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            _context.Comments.Remove(comment);
        }

        public async Task<bool> CommentExists(Guid commentId)
        {
            if (commentId == Guid.Empty) throw new ArgumentNullException(nameof(commentId));
            return await _context.Comments.AnyAsync(c => c.Id == commentId).ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync().ConfigureAwait(false)) >= 0);
        }



    }
}
