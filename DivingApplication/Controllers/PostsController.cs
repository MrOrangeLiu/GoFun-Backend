﻿using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Repositories.Posts;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using static DivingApplication.Entities.User;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;


        public PostsController(IPostRepository postRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetPosts")]
        public IActionResult GetPosts([FromQuery] PostResourceParameters postResourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<PostOutputDto, Post>(postResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var postsFromRepo = _postRepository.GetPosts(postResourceParameters);

            var previousPageLink = postsFromRepo.HasPrevious ? CreatePostsUri(postResourceParameters, UriType.PreviousPage, "GetPosts") : null;
            var nextPageLink = postsFromRepo.HasNext ? CreatePostsUri(postResourceParameters, UriType.NextPage, "GetPosts") : null;

            var metaData = new
            {
                totalCount = postsFromRepo.TotalCount,
                pageSize = postsFromRepo.PageSize,
                currentPage = postsFromRepo.CurrentPage,
                totalPages = postsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(postsFromRepo).ShapeData(postResourceParameters.Fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("/following", Name = "GetFollowingPosts")]
        public async Task<IActionResult> GetAllFollowingPosts([FromQuery] PostResourceParameters postResourceParameters)
        {
            if (!_propertyMapping.ValidMappingExist<PostOutputDto, Post>(postResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            var postsFromRepo = await _postRepository.GetAllFollowingPosts(userId, postResourceParameters);

            var previousPageLink = postsFromRepo.HasPrevious ? CreatePostsUri(postResourceParameters, UriType.PreviousPage, "GetFollowingPosts") : null;
            var nextPageLink = postsFromRepo.HasNext ? CreatePostsUri(postResourceParameters, UriType.NextPage, "GetFollowingPosts") : null;

            var metaData = new
            {
                totalCount = postsFromRepo.TotalCount,
                pageSize = postsFromRepo.PageSize,
                currentPage = postsFromRepo.CurrentPage,
                totalPages = postsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(postsFromRepo).ShapeData(postResourceParameters.Fields));


        }

        [AllowAnonymous]
        [HttpGet("{postId}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(Guid postId, [FromQuery]string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(fields)) return BadRequest();

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            return Ok(_mapper.Map<PostOutputDto>(postFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostForCreatingDto post, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(fields)) return BadRequest();

            var postEntity = _mapper.Map<Post>(post);

            await _postRepository.AddPost(postEntity, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            await _postRepository.Save();

            var postToReturn = _mapper.Map<PostOutputDto>(postEntity);

            return CreatedAtRoute("GetPost", new { postId = postToReturn.Id, fields }, postToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPatch("{postId}")]
        public async Task<IActionResult> PartiallyUpdatePost(Guid postId, [FromBody] JsonPatchDocument<PostUpdatingDto> patchDocument, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var postFromRepo = await _postRepository.GetPost(postId).ConfigureAwait(false);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            var postToPatch = _mapper.Map<PostUpdatingDto>(postFromRepo);
            patchDocument.ApplyTo(postToPatch, ModelState);

            if (!TryValidateModel(postToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(postToPatch, postFromRepo); // Overriding

            await _postRepository.UpdatePost(postFromRepo).ConfigureAwait(false);

            await _postRepository.Save().ConfigureAwait(false);

            var postToReturn = _mapper.Map<PostOutputDto>(postFromRepo);

            return CreatedAtRoute(
                    "GetPost",
                    new { postId = postToReturn.Id, fields },
                    postToReturn.ShapeData(fields)
                );
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var postFromRepo = await _postRepository.GetPost(postId).ConfigureAwait(false);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            _postRepository.DeletePost(postFromRepo);
            await _postRepository.Save().ConfigureAwait(false);

            return Ok(_mapper.Map<PostOutputDto>(postFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> UserLikePostToggle(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Checking if the post Exist

            var post = await _postRepository.GetPost(postId).ConfigureAwait(false);

            if (post == null) return NotFound();

            var currentPostLike = await _postRepository.GetCurrentUserPostLike(userId, postId);
            bool Adding;

            if (currentPostLike == null)
            {
                currentPostLike = await _postRepository.UserLikePost(userId, postId);
                await _postRepository.Save().ConfigureAwait(false);
                Adding = true;

            }
            else
            {
                _postRepository.UserUnlikeAPost(currentPostLike);
                await _postRepository.Save().ConfigureAwait(false);
                Adding = false;

            }

            return Ok(
                new
                {
                    Adding,
                    currentPostLike.UserId,
                    currentPostLike.PostId,
                }
                );
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("save/{postId}")]
        public async Task<IActionResult> UserSavePostToggle(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Checking if the post Exist

            var post = await _postRepository.GetPost(postId);

            if (post == null) return NotFound();

            var currentPostSave = await _postRepository.GetCurrentUserPostSave(userId, postId);
            bool Adding;

            if (currentPostSave == null)
            {
                currentPostSave = await _postRepository.UserSavePost(userId, postId);
                await _postRepository.Save();
                Adding = true;

            }
            else
            {
                _postRepository.UserUnlikeAPost(currentPostSave);
                await _postRepository.Save();
                Adding = false;

            }

            return Ok(
                new
                {
                    Adding,
                    currentPostSave.UserId,
                    currentPostSave.PostId,
                }
                );
        }


        private string CreatePostsUri(PostResourceParameters postResourceParameters, UriType uriType, string routeName)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = postResourceParameters.PageNumber - 1,
                        postResourceParameters.PageSize,
                        postResourceParameters.SearchQuery,
                        postResourceParameters.OrderBy,
                        postResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = postResourceParameters.PageNumber + 1,
                        postResourceParameters.PageSize,
                        searchQuery = postResourceParameters.SearchQuery,
                        postResourceParameters.OrderBy,
                        postResourceParameters.Fields
                    });
                default:
                    return Url.Link(routeName, new
                    {
                        postResourceParameters.PageNumber,
                        postResourceParameters.PageSize,
                        postResourceParameters.SearchQuery,
                        postResourceParameters.OrderBy,
                        postResourceParameters.Fields
                    });

            }
        }
    }
}
