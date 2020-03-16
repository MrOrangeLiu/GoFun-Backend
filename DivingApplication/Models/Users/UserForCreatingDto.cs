using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserForCreatingDto
    {
        public string Name { set; get; }
        public string Email { set; get; }
        public string Password { get; set; }
        public string UserGender { get; set; }

        public string Intro { get; set; }

    }
}
