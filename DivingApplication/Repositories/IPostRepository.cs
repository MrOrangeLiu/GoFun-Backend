using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
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
        Task<UserPostLike> GetCurrentUserPostLike(Guid userId, Guid postId);
        Task<UserPostSave> GetCurrentUserPostSave(Guid userId, Guid postId);
        Task<Post> GetPost(Guid postId);
        PageList<Post> GetPosts(PostResourceParameters postResourceParameters);
        Task<bool> PostExists(Guid postId);
        Task<bool> Save();
        Task UpdatePost(Post post);
        Task<UserPostLike> UserLikePost(Guid userId, Guid postId);
        void UserLikePostExist(Guid userId, Guid postId);
        Task<UserPostSave> UserSavePost(Guid userId, Guid postId);
        void UserUnlikeAPost(UserPostLike currentUserPostLike);
        void UserUnlikeAPost(UserPostSave currentUserPostSave);
    }
}