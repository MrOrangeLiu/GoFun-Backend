using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Models.Users;
using DivingApplication.Services;
using DivingApplication.Services.PropertyServices;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
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
        private readonly AppSettingsService _appSettings;



        public UserRepository(DivingAPIContext context, IPropertyMappingService propertyMapping, IOptions<AppSettingsService> appSettings)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _appSettings = appSettings.Value;

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
                                     .Include(c => c.CoachInfo)
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

            await _context.Users.AddRangeAsync(user);

        }

        public async Task<User> GetUser(Guid userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return _context.Users.Include(u => u.LikePosts) // What will happend?
                                     .Include(u => u.SavePosts)
                                     .Include(u => u.Followers)
                                     .Include(u => u.Following)
                                     .Include(u => u.OwingComments)
                                     .Include(u => u.OwningPosts)
                                     .Include(u => u.OwningServiceInfos)
                                     .Include(c => c.CoachInfo)
                                     .Include(u=> u.UserChatRooms)
                                     .SingleOrDefault(x => x.Id == userId);

            //return await _context.Users.FindAsync(userId);
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

            var user = await _context.Users.Include(u => u.OwningServiceInfos).FirstOrDefaultAsync(u => u.Id == userId);

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

            return await _context.Users.AnyAsync(u => u.Id == userId);
        }


        public async Task<bool> UserEmailExists(string email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
            // if the SaveChanges returns negative int, then it fail to save 
        }

        public bool SaveForJwt()
        {
            return (_context.SaveChanges() >= 0);
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

            await _context.AddRangeAsync(userFollow);
            return userFollow;
        }

        public void UserUnFollowUser(UserFollow currentUserFollow)
        {
            _context.Remove(currentUserFollow);
        }

        public async Task<IEnumerable<User>> GetAllFollowers(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Followers).SingleOrDefaultAsync(u => u.Id == userId);
            return user.Followers.Select(uf => uf.Follower).ToList();
        }


        public async Task<IEnumerable<User>> GetAllFollowing(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Following).SingleOrDefaultAsync(u => u.Id == userId);
            return user.Following.Select(uf => uf.Following).ToList();
        }

        public async Task<IEnumerable<Post>> GetAllSavePosts(Guid userId)
        {
            var user = await _context.Users.Include(u => u.SavePosts).SingleOrDefaultAsync(u => u.Id == userId);
            var allPostId = user.SavePosts.Select(sp => sp.PostId).ToList();
            return await _context.Posts.Where(p => allPostId.Contains(p.Id)).ToListAsync();
        }


        public async Task<IEnumerable<Post>> GetAllLikePosts(Guid userId)
        {
            var user = await _context.Users.Include(u => u.LikePosts).SingleOrDefaultAsync(u => u.Id == userId);
            var allPostId = user.LikePosts.Select(sp => sp.PostId).ToList();
            return await _context.Posts.Where(p => allPostId.Contains(p.Id)).ToListAsync();

        }

        public async Task<IEnumerable<Post>> GetAllOwningPost(Guid userId)
        {
            var user = await _context.Users.Include(u => u.OwningPosts).SingleOrDefaultAsync(u => u.Id == userId);
            return user.OwningPosts;
        }

        public async Task<CoachInfo> GetCoachInfoForUser(Guid userId)
        {
            var user = await _context.Users.Include(u => u.CoachInfo).FirstOrDefaultAsync(u => u.Id == userId);
            return user.CoachInfo;
        }


        public async Task<IEnumerable<ServiceInfo>> GetServiceInfoForUser(Guid userId)
        {
            var user = await _context.Users.Include(u => u.OwningServiceInfos).FirstOrDefaultAsync(u => u.Id == userId);


            return user.OwningServiceInfos;
        }

        public async Task SendVerificationEmail(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var sendingMessage = new MimeMessage
            {
                Sender = new MailboxAddress("DivingApp", "diving_app_2020@outlook.com"),
                Subject = "Diving App 驗證信",
            };


            sendingMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"</br></br><center><h1> 使用者 { user.Name } 您好 </h1></center></br><center><h3> 您的身分驗證碼為: {user.EmailVerificationCode} </ h3></center></br></br><center><p> Diving App 團隊 </p><p> 敬上<p> </center> "
            };

            sendingMessage.To.Add(new MailboxAddress(user.Email));


            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.MessageSent += (sender, args) => { };

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_appSettings.Email, _appSettings.EmailPassword);

                await smtp.SendAsync(sendingMessage);

                await smtp.DisconnectAsync(true);

            }


        }

        public async Task<PageList<User>> GetUsers(UserResourceParameterts userResourceParameters, Guid loginUserId)
        {
            var loginUser = await _context.Users
                .Include(u => u.UserChatRooms)
                .ThenInclude(c => c.ChatRoom)
                .ThenInclude(c => c.UserChatRooms)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Include(u => u.CoachInfo)
                .SingleOrDefaultAsync(u => u.Id == loginUserId);

            if (loginUser == null) throw new Exception(message: "Cannot find the user");

            if (userResourceParameters == null) throw new ArgumentNullException(nameof(userResourceParameters));

            var collection = _context.Users as IQueryable<User>;

            if (!string.IsNullOrWhiteSpace(userResourceParameters.SearchQuery))
            {
                string searchinQuery = userResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.Where(p => p.Name.ToLower().Contains(searchinQuery) || p.Email.Contains(searchinQuery));
            }

            var removedString = "familiarity";
            var indexOfHot = userResourceParameters.OrderBy.IndexOf(removedString, StringComparison.OrdinalIgnoreCase);
            userResourceParameters.OrderBy = (indexOfHot < 0) ? userResourceParameters.OrderBy : userResourceParameters.OrderBy.Remove(indexOfHot, removedString.Length);

            var stringNullTest = string.IsNullOrWhiteSpace(userResourceParameters?.OrderBy?.Replace(",", ""));

            if (!stringNullTest)
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<UserBriefOutputDto, User>();
                collection = collection.ApplySort(userResourceParameters.OrderBy, postPropertyMappingDictionary);
            }

            if (indexOfHot >= 0)
            {
                // Familiar Algorithm

                if (loginUser == null) throw new ArgumentNullException(nameof(loginUser));

                List<Guid> followingUsers = loginUser.Following.Select(f => f.FollowingId).ToList();
                List<Guid> followerUsers = loginUser.Followers.Select(f => f.FollowerId).ToList();
                List<Guid> chatRoomUsers = loginUser.UserChatRooms.SelectMany(c => c.ChatRoom.UserChatRooms.Select(ucr => ucr.UserId).Where(uid => uid != loginUser.Id)).ToList();


                collection = collection.OrderBy(u =>
                              (followingUsers.Contains(u.Id) ? 1 : 0) +
                              (followerUsers.Contains(u.Id) ? 1 : 0) +
                              (chatRoomUsers.Contains(u.Id) ? 1 : 0)
                              );

                // Checking if the user is the followers, followings or the one in the same chatRooms,
                //collection = collection.OrderBy(u =>
                //(
                //        loginUser.Following.Select(f => f.FollowingId).Contains(u.Id) ||
                //        loginUser.Followers.Select(f => f.FollowerId).Contains(u.Id)
                //                //loginUser.UserChatRooms.
                //                //        SelectMany(c => c.ChatRoom.UserChatRooms.Select(ucr => ucr.UserId).Where(uid => uid != loginUser.Id)).Contains(u.Id)
                //                )
                //                ? 0 : 1
                //                );

            } // it has familiarity sort

            // if it match an Email, put it at first
            if (!string.IsNullOrWhiteSpace(userResourceParameters.SearchQuery))
            {
                // We don't need exact match, only need familiarity
                string searchinQuery = userResourceParameters.SearchQuery.Trim().ToLower();
                collection = collection.OrderBy(u => u.Email.ToLower() == searchinQuery ? 0 : 1);
            }

            return PageList<User>.Create(collection, userResourceParameters.PageNumber, userResourceParameters.PageSize);
        }





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
