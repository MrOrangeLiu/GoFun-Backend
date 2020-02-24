using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserBriefOutputDto
    {
        public Guid Id { set; get; }

        public string Email { set; get; }

        public string Name { set; get; }

        public byte[] ProfileImage { set; get; }

        public string UserRole { get; set; }
        public string UserGender { get; set; }
        public DateTime LastSeen { get; set; }
        public int NumOfFollowers { get; set; }
        public int NumOfFollowing { get; set; }
        public int NumOfOwningPosts { get; set; }
    }
}
