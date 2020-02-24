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
        public string Name { set; get; }

        public byte[] ProfileImage { set; get; }

        [Required]
        [MinLength(6)]
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
        public Role UserRole { get; set; } = Role.EmailNotVerified;
        public Gender UserGender { get; set; } = Gender.Male;
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }
        public string EmailVerificationCode { get; set; }
        public List<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public List<UserFollow> Following { get; set; } = new List<UserFollow>();
        public List<Post> OwningPosts { get; set; } = new List<Post>();
        public List<UserChatRoom> UserChatRooms { get; set; } = new List<UserChatRoom>();
        public List<UserPostLike> LikePosts { get; set; } = new List<UserPostLike>();
        public List<UserPostSave> SavePosts { get; set; } = new List<UserPostSave>();
        public List<UserMessageLike> LikeMessages { get; set; } = new List<UserMessageLike>();
        public List<UserMessageRead> ReadMessages { get; set; } = new List<UserMessageRead>();
    }
}
