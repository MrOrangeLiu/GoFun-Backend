﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Services
{
    public class AppSettingsService
    {
        public string SecretForJwt { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
    }
}
