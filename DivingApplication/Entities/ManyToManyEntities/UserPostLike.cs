﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Entities.ManyToManyEntities
{
    public class UserPostLike
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Post Post { get; set; }
        public Guid PostId { get; set; }
    }
}
