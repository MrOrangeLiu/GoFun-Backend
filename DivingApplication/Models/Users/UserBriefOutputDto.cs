using DivingApplication.Models.CoachInfo;
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
        public string Intro { get; set; }

        public string UserRole { get; set; }
        public string UserGender { get; set; }
        public DateTime LastSeen { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int OwningPostsCount { get; set; }
        public int OwningServiceInfosCount { get; set; }
        public Guid CoachInfoId { get; set; }
    }
}
