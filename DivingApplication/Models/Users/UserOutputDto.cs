using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserOutputDto
    {
        public Guid Id { set; get; }

        public string Email { set; get; }

        public string Name { set; get; }

        public byte[] ProfileImage { set; get; }

        public string UserRole { get; set; }
        public string UserGender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }
        public List<Guid> Followers { get; set; } = new List<Guid>();
        public List<Guid> Following { get; set; } = new List<Guid>();
        public List<Guid> OwningPosts { get; set; } = new List<Guid>();
        public List<Guid> UserChatRooms { get; set; } = new List<Guid>();
        public List<Guid> LikePosts { get; set; } = new List<Guid>();
        public List<Guid> SavePosts { get; set; } = new List<Guid>();
        public List<Guid> OwingServiceInfos { get; set; } = new List<Guid>();
    }
}
