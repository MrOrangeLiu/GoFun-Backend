using DivingApplication.Models;
using DivingApplication.Models.Posts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static DivingApplication.Entities.Post;

namespace DivingApplication.Validations
{
    public class ContentURLAmountLimit : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the Instance
            var post = (PostForCreatingDto)validationContext.ObjectInstance;

            var contentType = (ContentType)Enum.Parse(typeof(ContentType), post.PostContentType);

            switch (contentType)
            {
                case ContentType.Image:
                    if (post.ContentURL.Count > 9) return new ValidationResult("Cannot have more than 9 images in a post", new[] { "PostForCreatingDto" });
                    break;

                case ContentType.Video:
                    if (post.ContentURL.Count > 1) return new ValidationResult("Cannot have more than 1 video in a post", new[] { "PostForCreatingDto" });
                    break;
                default:
                    return new ValidationResult("Doesn't support this content type", new[] { "PostForCreatingDto" });
            }


            return ValidationResult.Success;

        }
    }
}
