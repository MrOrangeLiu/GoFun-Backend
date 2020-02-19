using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities.ManyToManyEntities
{
    public class UserFollow
    {
        public Guid FollowerId { get; set; }
        public User Follower { get; set; }
        public Guid FollowingId { get; set; }
        public User Following { get; set; }
    }
}
