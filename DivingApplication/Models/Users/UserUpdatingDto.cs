using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Users
{
    public class UserUpdatingDto
    {
        [MinLength(1)]
        public string Name { set; get; }
        public byte[] ProfileImage { set; get; }
        public string UserGender { get; set; }
        [MaxLength(150)]
        public string Intro { get; set; }

    }
}
