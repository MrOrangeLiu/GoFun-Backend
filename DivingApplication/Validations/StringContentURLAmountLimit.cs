using DivingApplication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Post;

namespace DivingApplication.Validations
{
    public class StringContentURLAmountLimit : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the Instance
            var post = (Post)validationContext.ObjectInstance;

            var urlList = post.ContentURL.Split(new[] { urlSplittor }, StringSplitOptions.RemoveEmptyEntries).ToList();

            switch (post.PostContentType)
            {
                case ContentType.Image:
                    if (urlList.Count > 9) return new ValidationResult("Cannot have more than 9 images in a post", new[] { "PostForCreatingDto" });
                    break;

                case ContentType.video:
                    if (urlList.Count > 1) return new ValidationResult("Cannot have more than 1 video in a post", new[] { "PostForCreatingDto" });
                    break;
                default:
                    return new ValidationResult("Doesn't support this content type", new[] { "PostForCreatingDto" });
            }


            return ValidationResult.Success;

        }
    }
}
