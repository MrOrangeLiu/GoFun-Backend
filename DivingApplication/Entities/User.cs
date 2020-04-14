using DivingApplication.Entities.ManyToManyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities
{
    public class User
    {
        public User()
        {

        }

        public enum Role
        {
            Banned,
            EmailNotVerified,
            Normal,
            Coach,
            Admin,
        }

        public enum Gender
        {
            Male,
            Female,
        }


        [Key]
        public Guid Id { set; get; }

        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Required]
        [MaxLength(64)]
        [MinLength(1)]

        public string Name { set; get; }

        public byte[] ProfileImage { set; get; }

        [Required]
        [MinLength(6)]
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
        public Role UserRole { get; set; } = Role.EmailNotVerified;
        public CoachInfo CoachInfo { get; set; }

        [MaxLength(150)]
        public string Intro { get; set; } = "";

        public Gender UserGender { get; set; } = Gender.Male;
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }
        public string EmailVerificationCode { get; set; }
        public List<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public List<UserFollow> Following { get; set; } = new List<UserFollow>();
        public List<Post> OwningPosts { get; set; } = new List<Post>();
        public List<Comment> OwningComments { get; set; } = new List<Comment>();
        public List<UserChatRoom> UserChatRooms { get; set; } = new List<UserChatRoom>();
        public List<ChatRoom> OwningChatRoom { get; set; } = new List<ChatRoom>();
        public List<UserPostLike> LikePosts { get; set; } = new List<UserPostLike>();
        public List<UserPostSave> SavePosts { get; set; } = new List<UserPostSave>();
        public List<UserMessageLike> LikeMessages { get; set; } = new List<UserMessageLike>();
        public List<UserMessageRead> ReadMessages { get; set; } = new List<UserMessageRead>();
        public List<ServiceInfo> OwningServiceInfos { get; set; } = new List<ServiceInfo>();
        public List<UserPostTag> PostsTaggedMe { get; set; } = new List<UserPostTag>();
        public List<Report> OwningReports { get; set; } = new List<Report>();


        //public List<ChatRoomInviteUser> ChatRoomInvitations { get; set; } = new List<ChatRoomInviteUser>();
        //public List<ChatRoomAdminUser> AdminChatRooms { get; set; } = new List<ChatRoomAdminUser>();
    }
}
