﻿using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Posts;
using DivingApplication.Repositories;
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


        public PostsController(IPostRepository postRepository, IMapper mapper, IPropertyMappingService propertyMapping)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetPosts")]
        public IActionResult GetPosts([FromQuery] PostResourceParameters postResourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<PostOutpuDto, Post>(postResourceParameters.OrderBy)) return Ok();
            if (!_propertyValidation.HasValidProperties<PostOutpuDto>(postResourceParameters.Fields)) return Ok();

            var postsFromRepo = _postRepository.GetPosts(postResourceParameters);

            var previousPageLink = postsFromRepo.HasPrevious ? CreatePostsUri(postResourceParameters, UriType.PreviousPage) : null;
            var nextPageLink = postsFromRepo.HasNext ? CreatePostsUri(postResourceParameters, UriType.NextPage) : null;

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

            return Ok(_mapper.Map<IEnumerable<PostOutpuDto>>(postsFromRepo).ShapeData(postResourceParameters.Fields));
        }

        [AllowAnonymous]
        [HttpGet("{postId}", Name = "GetPost")]
        public async Task<IActionResult> GetBand(Guid postId, [FromQuery]string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutpuDto>(fields)) return BadRequest();

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            return Ok(_mapper.Map<PostOutpuDto>(postFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "NormalAndAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostForCreatingDto post, [FromQuery] string fields)
        {
            var postEntity = _mapper.Map<Post>(post);

            await _postRepository.AddPost(postEntity, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            await _postRepository.Save();

            var postToReturn = _mapper.Map<PostOutpuDto>(postEntity);

            return CreatedAtRoute("GetPost", new { postId = postToReturn.Id, fields }, postToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "NormalAndAdmin")]
        [HttpPatch("{postId}")]
        public async Task<IActionResult> PartiallyUpdatePost(Guid postId, [FromBody] JsonPatchDocument<PostUpdatingDto> patchDocument, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && userRole.ToLower() != Role.Admin.ToString().ToLower()) return Unauthorized();

            var postToPatch = _mapper.Map<PostUpdatingDto>(postFromRepo);
            patchDocument.ApplyTo(postToPatch, ModelState);

            if (!TryValidateModel(postToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(postToPatch, postFromRepo); // Overriding

            _postRepository.UpdatePost(postFromRepo);

            await _postRepository.Save();

            var postToReturn = _mapper.Map<PostOutpuDto>(postFromRepo);

            return CreatedAtRoute(
                    "GetPost",
                    new { postId = postToReturn.Id, fields },
                    postToReturn.ShapeData(fields)
                );
        }

        [Authorize(Policy = "NormalAndAdmin")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid postId, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && userRole.ToLower() != Role.Admin.ToString().ToLower()) return Unauthorized();

            _postRepository.DeletePost(postFromRepo);
            await _postRepository.Save();

            return Ok(_mapper.Map<PostOutpuDto>(postFromRepo).ShapeData(fields));
        }


        private string CreatePostsUri(PostResourceParameters postResourceParameters, UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetPosts", new
                    {
                        pageNumber = postResourceParameters.PageNumber - 1,
                        pageSize = postResourceParameters.PageSize,
                        searchQuery = postResourceParameters.SearchQuery,
                        OrderBy = postResourceParameters.OrderBy,
                        Fields = postResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link("GetPosts", new
                    {
                        pageNumber = postResourceParameters.PageNumber + 1,
                        pageSize = postResourceParameters.PageSize,
                        searchQuery = postResourceParameters.SearchQuery,
                        OrderBy = postResourceParameters.OrderBy,
                        Fields = postResourceParameters.Fields
                    });
                default:
                    return Url.Link("GetPosts", new
                    {
                        pageNumber = postResourceParameters.PageNumber,
                        pageSize = postResourceParameters.PageSize,
                        searchQuery = postResourceParameters.SearchQuery,
                        OrderBy = postResourceParameters.OrderBy,
                        Fields = postResourceParameters.Fields
                    });

            }
        }
    }
}
