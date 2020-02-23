using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories
{
    public interface IPostRepository
    {
        Task AddPost(Post post, Guid authorId);
        void DeletePost(Post post);
        Task<Post> GetPost(Guid postId);
        PageList<Post> GetPosts(PostResourceParameters postResourceParameters);
        Task<bool> PostExists(Guid postId);
        Task<bool> Save();
        Task UpdatePost(Post post);
    }
}