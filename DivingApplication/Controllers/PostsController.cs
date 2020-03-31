using AutoMapper;
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
        public async Task<IActionResult> GetPosts([FromQuery] PostResourceParametersWithOrderBy postResourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<PostOutputDto, Post>(postResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var postsFromRepo = await _postRepository.GetPosts(postResourceParameters);

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


        [AllowAnonymous]
        [HttpGet("hot", Name = "GetHotPosts")]
        public async Task<IActionResult> GetHotPosts([FromQuery] PostResourceParametersForHot postResourceParameters)
        {
            // No orderBy Options in the postResourceParameters
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var postsFromRepo = await _postRepository.GetHotPosts(postResourceParameters); // the orderBy property will be ignore

            var previousPageLink = postsFromRepo.HasPrevious ? CreatePostsUri(postResourceParameters, UriType.PreviousPage, "GetHotPosts") : null;
            var nextPageLink = postsFromRepo.HasNext ? CreatePostsUri(postResourceParameters, UriType.NextPage, "GetHotPosts") : null;

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
        [HttpGet("nearby", Name = "GetNearbyPosts")]
        public async Task<IActionResult> GetNearbyPosts([FromQuery] PostResourceParametersForNearby postResourceParameters)
        {
            // No orderBy Options in the postResourceParameters
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var postsFromRepo = await _postRepository.GetNearbyPosts(postResourceParameters); // the orderBy property will be ignore

            var previousPageLink = postsFromRepo.HasPrevious ? CreatePostsUri(postResourceParameters, UriType.PreviousPage, "GetNearbyPosts") : null;
            var nextPageLink = postsFromRepo.HasNext ? CreatePostsUri(postResourceParameters, UriType.NextPage, "GetNearbyPosts") : null;

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

        [Authorize(Policy = "c")]
        [HttpGet("following", Name = "GetFollowingPosts")]
        public async Task<IActionResult> GetAllFollowingPosts([FromQuery] PostResourceParametersWithOrderBy postResourceParameters)
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

            // Adding Tags
            foreach (var pt in post.PostTopicsIds)
            {
                postEntity.PostTopics.Add(new Entities.ManyToManyEntities.PostTopic
                {
                    PostId = postEntity.Id,
                    TopicId = Guid.Parse(pt)
                });
            }

            foreach (var tu in post.TaggedUsersIds)
            {
                postEntity.TaggedUsers.Add(new Entities.ManyToManyEntities.UserPostTag
                {
                    PostId = postEntity.Id,
                    UserId = Guid.Parse(tu)
                });
            }

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

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            var postToPatch = _mapper.Map<PostUpdatingDto>(postFromRepo);
            patchDocument.ApplyTo(postToPatch, ModelState);

            if (!TryValidateModel(postToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(postToPatch, postFromRepo); // Overriding

            await _postRepository.UpdatePost(postFromRepo);

            await _postRepository.Save();

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

            var postFromRepo = await _postRepository.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            _postRepository.DeletePost(postFromRepo);
            await _postRepository.Save();

            return Ok(_mapper.Map<PostOutputDto>(postFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> UserLikePostToggle(Guid postId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Checking if the post Exist

            var post = await _postRepository.GetPost(postId);

            if (post == null) return NotFound();

            var currentPostLike = await _postRepository.GetCurrentUserPostLike(userId, postId);
            bool Adding;

            if (currentPostLike == null)
            {
                currentPostLike = await _postRepository.UserLikePost(userId, postId);
                await _postRepository.Save();
                Adding = true;

            }
            else
            {
                _postRepository.UserUnlikeAPost(currentPostLike);
                await _postRepository.Save();
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


        private string CreatePostsUri(IPostResourceParameters postResourceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, postResourceParameters.CreateUrlParameters(uriType));
        }
    }
}
