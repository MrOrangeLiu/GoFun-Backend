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
using DivingApplication.Models.CoachInfo;
using DivingApplication.Repositories.CoachInfos;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static DivingApplication.Entities.User;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachInfosController : ControllerBase
    {
        private readonly ICoachInfosRepository _coachInfosRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;

        public CoachInfosController(ICoachInfosRepository coachInfosRepository,
                                    IMapper mapper,
                                    IPropertyMappingService propertyMapping,
                                    IPropertyValidationService propertyValidation)
        {
            _coachInfosRepository = coachInfosRepository;
            _mapper = mapper;
            _propertyMapping = propertyMapping;
            _propertyValidation = propertyValidation;
        }


        // Get


        [AllowAnonymous]
        [HttpGet(Name = "GetCoachInfos")]
        public IActionResult GetCoachInfos([FromQuery] CoachInfoResourceParameters coachInfoReposrceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<CoachInfoOutputDto, CoachInfo>(coachInfoReposrceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(coachInfoReposrceParameters.Fields)) return BadRequest();

            var coachInfoFromRepo = _coachInfosRepository.GetCoachInfos(coachInfoReposrceParameters);

            var previousPageLink = coachInfoFromRepo.HasPrevious ? CreateCoachInfoUri(coachInfoReposrceParameters, UriType.PreviousPage, "GetCoachInfos") : null;
            var nextPageLink = coachInfoFromRepo.HasNext ? CreateCoachInfoUri(coachInfoReposrceParameters, UriType.NextPage, "GetCoachInfos") : null;

            var metaData = new
            {
                totalCount = coachInfoFromRepo.TotalCount,
                pageSize = coachInfoFromRepo.PageSize,
                currentPage = coachInfoFromRepo.CurrentPage,
                totalPages = coachInfoFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<CoachInfoOutputDto>>(coachInfoFromRepo).ShapeData(coachInfoReposrceParameters.Fields));
        }

        [AllowAnonymous]
        [HttpGet("{coachInfoId}", Name = "GetCoachInfo")]
        public async Task<IActionResult> GetCoachInfo(Guid coachInfoId, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(fields)) return BadRequest();

            var coachInfoFromRepo = await _coachInfosRepository.GetCoachInfo(coachInfoId).ConfigureAwait(false);

            var coachInfoToReturn = _mapper.Map<CoachInfoOutputDto>(coachInfoFromRepo);

            return Ok(coachInfoToReturn.ShapeData(fields));
        }



        [Authorize(Policy = "CoachAndAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoachInfo([FromBody] CoachInfoForCreatingDto coachInfo, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(fields)) return BadRequest();

            var coachInfoEntity = _mapper.Map<CoachInfo>(coachInfo);

            await _coachInfosRepository.AddCoachInfo(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)), coachInfoEntity).ConfigureAwait(false);
            await _coachInfosRepository.Save().ConfigureAwait(false);

            var coachInfoToReturn = _mapper.Map<CoachInfoOutputDto>(coachInfoEntity);

            return CreatedAtRoute("GetCoachInfo", new { coachInfoId = coachInfoToReturn.Id, fields }, coachInfoToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "CoachAndAdmin")]
        [HttpPatch("{coachInfoId}")]
        public async Task<IActionResult> PartiallyUpdateCoachInfo(Guid coachInfoId, [FromBody] JsonPatchDocument<CoachInfoUpdatingDto> patchDocument, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(fields)) return BadRequest();

            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var coachInfoFromRepo = await _coachInfosRepository.GetCoachInfo(coachInfoId).ConfigureAwait(false);

            if (coachInfoFromRepo == null) return NotFound();

            if (logginUserId != coachInfoFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            var coachInfoToPatch = _mapper.Map<CoachInfoUpdatingDto>(coachInfoFromRepo);
            patchDocument.ApplyTo(coachInfoToPatch, ModelState);

            if (!TryValidateModel(coachInfoToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(coachInfoToPatch, coachInfoFromRepo); // Overriding

            _coachInfosRepository.UpdateCoachInfo(coachInfoFromRepo);

            await _coachInfosRepository.Save().ConfigureAwait(false);

            var postToReturn = _mapper.Map<CoachInfoOutputDto>(coachInfoFromRepo);

            return CreatedAtRoute(
                    "GetCoachInfo",
                    new { coachInfoId = postToReturn.Id, fields },
                    postToReturn.ShapeData(fields)
                );
        }



        [Authorize(Policy = "CoachAndAdmin")]
        [HttpDelete("{coachInfoId}")]
        public async Task<IActionResult> DeleteCoachInfo(Guid coachInfoId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(fields)) return BadRequest();

            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var postFromRepo = await _coachInfosRepository.GetCoachInfo(coachInfoId).ConfigureAwait(false);

            if (postFromRepo == null) return NotFound();

            if (logginUserId != postFromRepo.AuthorId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            _coachInfosRepository.DeleteCoachInfo(postFromRepo);
            await _coachInfosRepository.Save().ConfigureAwait(false);

            return Ok(_mapper.Map<CoachInfoOutputDto>(postFromRepo).ShapeData(fields));
        }


        private string CreateCoachInfoUri(CoachInfoResourceParameters coachInfoResourceParameters, UriType uriType, string routeName)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = coachInfoResourceParameters.PageNumber - 1,
                        coachInfoResourceParameters.PageSize,
                        coachInfoResourceParameters.SearchQuery,
                        coachInfoResourceParameters.OrderBy,
                        coachInfoResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = coachInfoResourceParameters.PageNumber + 1,
                        coachInfoResourceParameters.PageSize,
                        searchQuery = coachInfoResourceParameters.SearchQuery,
                        coachInfoResourceParameters.OrderBy,
                        coachInfoResourceParameters.Fields
                    });
                default:
                    return Url.Link(routeName, new
                    {
                        coachInfoResourceParameters.PageNumber,
                        coachInfoResourceParameters.PageSize,
                        coachInfoResourceParameters.SearchQuery,
                        coachInfoResourceParameters.OrderBy,
                        coachInfoResourceParameters.Fields
                    });

            }
        }

    }
}