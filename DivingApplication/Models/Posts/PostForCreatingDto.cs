﻿using DivingApplication.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Models.Posts
{
    [ContentURLAmountLimit(ErrorMessage = "The ContentType or PostContentURL is not valid")]
    public class PostForCreatingDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(64)]
        public string Title { get; set; }
        [MaxLength(2048)]
        public string Description { get; set; }
        [Required]
        public string PostContentType { get; set; }

        [Required]
        public List<string> ContentURL { get; set; } = new List<string>();
    }
}
