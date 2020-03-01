using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Comments
{
    public interface ICommentRepository
    {
        Task AddComment(Comment comment, Guid postId, Guid authorId);
        Task<bool> CommentExists(Guid commentId);
        void DeleteComment(Comment comment);
        Task<Comment> GetComment(Guid commentId);
        PageList<Comment> GetCommentsForPost(Guid postId, CommentResourceParameters commentResourceParameters);
        Task<bool> Save();
        void UpdateComment(Comment comment);
    }
}