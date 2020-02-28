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
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;


        public UserRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        public User Authenticate(string Email, string password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.Include(u => u.LikePosts) // What will happend?
                                     .Include(u => u.SavePosts)
                                     .Include(u => u.Followers)
                                     .Include(u => u.Following)
                                     .Include(u => u.OwingComments)
                                     .Include(u => u.OwningPosts)
                                     .Include(u => u.OwningServiceInfos)
                                     .SingleOrDefault(x => x.Email == Email);

            // check if Email exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public async Task AddUser(User user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            if (password == null) throw new ArgumentNullException(nameof(password));

            // Hashing the Password
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = DateTime.Now;
            user.LastSeen = DateTime.Now;

            await _context.Users.AddRangeAsync(user).ConfigureAwait(false);

        }

        public async Task<User> GetUser(Guid userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return await _context.Users.FindAsync(userId);
        }


        public User GetUserForJwt(Guid userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return _context.Users.Find(userId);
        }

        public async Task<List<ServiceInfo>> GetServiceInfosForUser(Guid userId)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var user = await _context.Users.Include(u => u.OwningServiceInfos).FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);

            return user.OwningServiceInfos;
        }


        public async Task UpdateUser(User user)
        {
        }

        public void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);

        }

        public async Task<bool> UserExists(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            return await _context.Users.AnyAsync(u => u.Id == userId).ConfigureAwait(false);
        }


        public async Task<bool> UserEmailExists(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            return await _context.Users.AnyAsync(u => u.Email == email).ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync().ConfigureAwait(false)) >= 0);
            // if the SaveChanges returns negative int, then it fail to save 
        }


        public async Task<UserFollow> GetCurrentUserFollow(Guid followerId, Guid followingId)
        {
            if (followerId == Guid.Empty) throw new ArgumentNullException(nameof(followerId));
            if (followingId == Guid.Empty) throw new ArgumentNullException(nameof(followingId));

            return (UserFollow)await _context.FindAsync(typeof(UserFollow), followerId, followingId);
        }

        public async Task<UserFollow> UserFollowUser(Guid followerId, Guid followingId)
        {
            if (followerId == Guid.Empty) throw new ArgumentNullException(nameof(followerId));
            if (followingId == Guid.Empty) throw new ArgumentNullException(nameof(followingId));

            var userFollow = new UserFollow
            {
                FollowerId = followerId,
                FollowingId = followingId,
            };

            await _context.AddRangeAsync(userFollow).ConfigureAwait(false);
            return userFollow;
        }

        public void UserUnFollowUser(UserFollow currentUserFollow)
        {
            _context.Remove(currentUserFollow);
        }

        public async Task<IEnumerable<User>> GetAllFollowers(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Followers).SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            return user.Followers.Select(uf => uf.Follower).ToList();
        }


        public async Task<IEnumerable<User>> GetAllFollowing(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Following).SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            return user.Following.Select(uf => uf.Following).ToList();
        }

        public async Task<IEnumerable<Post>> GetAllSavePosts(Guid userId)
        {
            var user = await _context.Users.Include(u => u.SavePosts).SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            var allPostId = user.SavePosts.Select(sp => sp.PostId).ToList();
            return await _context.Posts.Where(p => allPostId.Contains(p.Id)).ToListAsync().ConfigureAwait(false);
        }


        public async Task<IEnumerable<Post>> GetAllLikePosts(Guid userId)
        {
            var user = await _context.Users.Include(u => u.LikePosts).SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            var allPostId = user.LikePosts.Select(sp => sp.PostId).ToList();
            return await _context.Posts.Where(p => allPostId.Contains(p.Id)).ToListAsync().ConfigureAwait(false);

        }

        public async Task<IEnumerable<Post>> GetAllOwningPost(Guid userId)
        {
            var user = await _context.Users.Include(u => u.OwningPosts).SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            return user.OwningPosts;
        }

        public async Task<CoachInfo> GetCoachInfoForUser(Guid userId)
        {
            var user = await _context.Users.Include(u => u.CoachInfo).FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            return user.CoachInfo;
        }


        public async Task<IEnumerable<ServiceInfo>> GetServiceInfoForUser(Guid userId)
        {
            var user = await _context.Users.Include(u => u.OwningServiceInfos).FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
            return user.OwningServiceInfos;
        }

        //public async Task RemoveAllUserFollow()
        //{
        //    var users = await _context.Users.Include(u => u.Followers).ToListAsync();

        //    foreach (var u in users)
        //    {
        //        foreach (var f in u.Followers)
        //        {
        //            var uf = (UserFollow)await _context.FindAsync(typeof(UserFollow), f.FollowerId, f.FollowingId);

        //            if (uf != null)
        //            {
        //                _context.Remove(uf);
        //            }

        //        }

        //        foreach (var f in u.Following)
        //        {
        //            var uf = (UserFollow)await _context.FindAsync(typeof(UserFollow), f.FollowerId, f.FollowingId);

        //            if (uf != null)
        //            {
        //                _context.Remove(uf);
        //            }

        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}


        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
