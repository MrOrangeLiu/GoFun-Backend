using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserForCreatingDto
    {
        [MinLength(1)]
        public string Name { set; get; }
        public string Email { set; get; }
        [MinLength(6)]
        public string Password { get; set; }
        public string UserGender { get; set; }
        [MaxLength(150)]
        public string Intro { get; set; }

    }
}
