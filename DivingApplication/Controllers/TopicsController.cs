using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Topic;
using DivingApplication.Repositories.Topics;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {

        private readonly ITopicsRepository _topicsRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;

        public TopicsController(ITopicsRepository topicsRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _topicsRepository = topicsRepository ?? throw new ArgumentNullException(nameof(topicsRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetTopics")]
        public IActionResult GetTopics([FromQuery] TopicResourceParameters topicResourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<TopicOutputDto, Topic>(topicResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<TopicOutputDto>(topicResourceParameters.Fields)) return BadRequest();

            var topicFromRepo = _topicsRepository.GetTopics(topicResourceParameters);

            var previousPageLink = topicFromRepo.HasPrevious ? CreatePostsUri(topicResourceParameters, UriType.PreviousPage, "GetTopics") : null;
            var nextPageLink = topicFromRepo.HasNext ? CreatePostsUri(topicResourceParameters, UriType.NextPage, "GetTopics") : null;

            var metaData = new
            {
                totalCount = topicFromRepo.TotalCount,
                pageSize = topicFromRepo.PageSize,
                currentPage = topicFromRepo.CurrentPage,
                totalPages = topicFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<TopicOutputDto>>(topicFromRepo).ShapeData(topicResourceParameters.Fields));
        }



        [AllowAnonymous]
        [HttpGet("{topicId}", Name = "GetTopic")]
        public async Task<IActionResult> GetTopic(Guid topicId, [FromQuery]string fields)
        {
            if (!_propertyValidation.HasValidProperties<TopicOutputDto>(fields)) return BadRequest();

            var topicFromRepo = await _topicsRepository.GetTopic(topicId).ConfigureAwait(false);

            if (topicFromRepo == null) return NotFound();

            return Ok(_mapper.Map<TopicOutputDto>(topicFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicForCreatingDto topic, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<TopicOutputDto>(fields)) return BadRequest();

            if (await _topicsRepository.TopicWithNameExists(topic.Name).ConfigureAwait(false)) return BadRequest();

            var topicEntity = _mapper.Map<Topic>(topic);

            await _topicsRepository.AddTopic(topicEntity).ConfigureAwait(false);

            await _topicsRepository.Save().ConfigureAwait(false);

            var topicToReturn = _mapper.Map<TopicOutputDto>(topicEntity);

            return CreatedAtRoute("GetTopic", new { topicId = topicToReturn.Id, fields }, topicToReturn.ShapeData(fields));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{topicId}")]
        public async Task<IActionResult> PartiallyUpdateTopic(Guid topicId, [FromBody] JsonPatchDocument<TopicUpdatingDto> patchDocument, [FromQuery] string fields)
        {

            // Usually We don't want to change the name of Topics

            var topicFromRepo = await _topicsRepository.GetTopic(topicId).ConfigureAwait(false);

            if (topicFromRepo == null) return NotFound();

            var postToPatch = _mapper.Map<TopicUpdatingDto>(topicFromRepo);
            patchDocument.ApplyTo(postToPatch, ModelState);

            if (!TryValidateModel(postToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(postToPatch, topicFromRepo); // Overriding

            await _topicsRepository.UpdateTopic(topicFromRepo).ConfigureAwait(false);

            await _topicsRepository.Save().ConfigureAwait(false);

            var topicToReturn = _mapper.Map<TopicOutputDto>(topicFromRepo);

            return CreatedAtRoute(
                    "GetTopic",
                    new { topicId = topicToReturn.Id, fields },
                    topicToReturn.ShapeData(fields)
                );
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteTopic(Guid topicId, [FromQuery] string fields)
        {

            var topicToReturn = await _topicsRepository.GetTopic(topicId).ConfigureAwait(false);

            if (topicToReturn == null) return NotFound();

            _topicsRepository.DeleteTopic(topicToReturn);
            await _topicsRepository.Save().ConfigureAwait(false);

            return Ok(_mapper.Map<TopicOutputDto>(topicToReturn).ShapeData(fields));
        }


        private string CreatePostsUri(TopicResourceParameters topicResourceParameter, UriType uriType, string routeName)
        {
            return Url.Link(routeName, topicResourceParameter.CreateUrlParameters(uriType));
        }
    }
}