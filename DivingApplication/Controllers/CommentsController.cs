using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Comments;
using DivingApplication.Repositories.Comments;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static DivingApplication.Entities.User;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;

        public CommentsController(ICommentRepository commentRepository,
                                  IMapper mapper,
                                  IPropertyMappingService propertyMapping,
                                  IPropertyValidationService propertyValidation)
        {
            _commentRepository = commentRepository;
            _mapper = mapper;
            _propertyMapping = propertyMapping;
            _propertyValidation = propertyValidation;
        }

        [AllowAnonymous]
        [HttpGet("post/{postId}", Name = "GetCommentsForPost")]
        public async Task<IActionResult> GetCommentsForPost([FromQuery] CommentResourceParameters commentResourceParameters, Guid postId)
        {
            if (!_propertyMapping.ValidMappingExist<CommentOutputDto, Comment>(commentResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<CommentOutputDto>(commentResourceParameters.Fields)) return BadRequest();

            var commentsFromRepo = _commentRepository.GetCommentsForPost(postId, commentResourceParameters);

            var previousPageLink = commentsFromRepo.HasPrevious ? CreateCommentsUri(commentResourceParameters, UriType.PreviousPage, "GetCommentsForPost") : null;
            var nextPageLink = commentsFromRepo.HasNext ? CreateCommentsUri(commentResourceParameters, UriType.NextPage, "GetCommentsForPost") : null;

            var metaData = new
            {
                totalCount = commentsFromRepo.TotalCount,
                pageSize = commentsFromRepo.PageSize,
                currentPage = commentsFromRepo.CurrentPage,
                totalPages = commentsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<CommentOutputDto>>(commentsFromRepo).ShapeData(commentResourceParameters.Fields));
        }

        [AllowAnonymous]
        [HttpGet("{commentId}", Name = "GetComment")]
        public async Task<IActionResult> GetComment(Guid commentId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<CommentOutputDto>(fields)) return BadRequest();

            var commentFromRepo = await _commentRepository.GetComment(commentId);

            if (commentFromRepo == null) return NotFound();

            return Ok(_mapper.Map<CommentOutputDto>(commentFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("{postId}")]
        public async Task<IActionResult> CreateComment([FromBody] CommentForCreatingDto comment, [FromQuery] string fields, Guid postId)
        {
            var commentEntity = _mapper.Map<Comment>(comment);

            await _commentRepository.AddComment(commentEntity, postId, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).ConfigureAwait(false);
            await _commentRepository.Save().ConfigureAwait(false);

            var commentToReturn = _mapper.Map<CommentOutputDto>(commentEntity);

            return CreatedAtRoute("GetComment", new { commentId = commentToReturn.Id, fields }, commentToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPatch("{commentId}")]
        public async Task<IActionResult> PartiallyUpdateComment(Guid commentId,
                                                                [FromBody] JsonPatchDocument<CommentUpdatingDto> patchDocument,
                                                                [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var commentFromRepo = await _commentRepository.GetComment(commentId).ConfigureAwait(false);

            if (commentFromRepo == null) return NotFound();

            if (logginUserId != commentFromRepo.AuthorId && userRole.ToLower() != Role.Admin.ToString().ToLower()) return Unauthorized();

            var commentToPatch = _mapper.Map<CommentUpdatingDto>(commentFromRepo);
            patchDocument.ApplyTo(commentToPatch, ModelState);

            if (!TryValidateModel(commentToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(commentToPatch, commentFromRepo); // Overriding

            _commentRepository.UpdateComment(commentFromRepo);

            await _commentRepository.Save().ConfigureAwait(false);

            var commentToReturn = _mapper.Map<CommentOutputDto>(commentFromRepo);

            return CreatedAtRoute(
                    "GetComment",
                    new { commentId = commentToReturn.Id, fields },
                    commentToReturn.ShapeData(fields)
                );
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var commentFromRepo = await _commentRepository.GetComment(commentId).ConfigureAwait(false);

            if (commentFromRepo == null) return NotFound();

            _commentRepository.DeleteComment(commentFromRepo);
            await _commentRepository.Save().ConfigureAwait(false);

            return Ok(_mapper.Map<CommentOutputDto>(commentFromRepo).ShapeData(fields));
        }



        private string CreateCommentsUri(CommentResourceParameters commentResourceParameters, UriType uriType, string routeName)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = commentResourceParameters.PageNumber - 1,
                        commentResourceParameters.PageSize,
                        commentResourceParameters.OrderBy,
                        commentResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = commentResourceParameters.PageNumber + 1,
                        commentResourceParameters.PageSize,
                        commentResourceParameters.OrderBy,
                        commentResourceParameters.Fields
                    });
                default:
                    return Url.Link(routeName, new
                    {
                        commentResourceParameters.PageNumber,
                        commentResourceParameters.PageSize,
                        commentResourceParameters.OrderBy,
                        commentResourceParameters.Fields
                    });

            }
        }


    }
}