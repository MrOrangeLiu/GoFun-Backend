using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Posts
{
    public interface IPostRepository
    {
        Task AddPost(Post post, Guid authorId);
        void DeletePost(Post post);
        Task<PageList<Post>> GetAllFollowingPosts(Guid userId, PostResourceParametersWithOrderBy postResourceParameters);
        Task<UserPostLike> GetCurrentUserPostLike(Guid userId, Guid postId);
        Task<UserPostSave> GetCurrentUserPostSave(Guid userId, Guid postId);
        Task<PageList<Post>> GetHotPosts(PostResourceParametersForHot postResourceParameters);
        Task<PageList<Post>> GetNearbyPosts(PostResourceParametersForNearby postResourceParameters);
        Task<Post> GetPost(Guid postId);
        Task<PageList<Post>> GetPosts(PostResourceParametersWithOrderBy postResourceParameters);
        Task<bool> PostExists(Guid postId);
        Task<bool> Save();
        Task UpdatePost(Post post);
        Task<UserPostLike> UserLikePost(Guid userId, Guid postId);
        Task<UserPostSave> UserSavePost(Guid userId, Guid postId);
        void UserUnlikeAPost(UserPostLike currentUserPostLike);
        void UserUnlikeAPost(UserPostSave currentUserPostSave);
    }
}