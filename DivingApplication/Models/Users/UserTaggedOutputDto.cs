using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserTaggedOutputDto
    {
        public Guid Id { set; get; }

        public string Email { set; get; }

        public string Name { set; get; }
    }
}
