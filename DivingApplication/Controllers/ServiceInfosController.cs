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
using DivingApplication.Models.ServiceInfo;
using DivingApplication.Repositories.ServiceInfos;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static DivingApplication.Entities.User;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceInfosController : ControllerBase
    {
        private readonly IServiceInfosRepository _serviceInfoRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;


        public ServiceInfosController(IServiceInfosRepository serviceInfoRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceInfoRepository = serviceInfoRepository ?? throw new ArgumentNullException(nameof(serviceInfoRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetServiceInfos")]
        public IActionResult GetServiceInfos([FromQuery] ServiceInfoResourceParameters postResourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<ServiceInfoOutputDto, ServiceInfo>(postResourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<ServiceInfoOutputDto>(postResourceParameters.Fields)) return BadRequest();

            var serviceInfoFromRepo = _serviceInfoRepository.GetServiceInfos(postResourceParameters);

            var previousPageLink = serviceInfoFromRepo.HasPrevious ? CreateServiceInfosUri(postResourceParameters, UriType.PreviousPage, "GetServiceInfos") : null;
            var nextPageLink = serviceInfoFromRepo.HasNext ? CreateServiceInfosUri(postResourceParameters, UriType.NextPage, "GetServiceInfos") : null;

            var metaData = new
            {
                totalCount = serviceInfoFromRepo.TotalCount,
                pageSize = serviceInfoFromRepo.PageSize,
                currentPage = serviceInfoFromRepo.CurrentPage,
                totalPages = serviceInfoFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<ServiceInfoOutputDto>>(serviceInfoFromRepo).ShapeData(postResourceParameters.Fields));
        }



        [AllowAnonymous]
        [HttpGet("{serviceInfoId}", Name = "GetServiceInfo")]
        public async Task<IActionResult> GetServiceInfo(Guid serviceInfoId, [FromQuery]string fields)
        {
            if (!_propertyValidation.HasValidProperties<ServiceInfoOutputDto>(fields)) return BadRequest();

            var postFromRepo = await _serviceInfoRepository.GetServiceInfo(serviceInfoId).ConfigureAwait(false);

            if (postFromRepo == null) return NotFound();

            return Ok(_mapper.Map<ServiceInfoOutputDto>(postFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost]
        public async Task<IActionResult> CreateServiceInfo([FromBody] ServiceInfoForCreatingDto post, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<ServiceInfoOutputDto>(fields)) return BadRequest();

            var postEntity = _mapper.Map<ServiceInfo>(post);

            await _serviceInfoRepository.AddServiceInfo(postEntity, Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).ConfigureAwait(false);
            await _serviceInfoRepository.Save().ConfigureAwait(false);

            var serviceInfoToReturn = _mapper.Map<ServiceInfoOutputDto>(postEntity);

            return CreatedAtRoute("GetServiceInfo", new { serviceInfoId = serviceInfoToReturn.Id, fields }, serviceInfoToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPatch("{serviceInfoId}")]
        public async Task<IActionResult> PartiallyUpdateServiceInfo(Guid serviceInfoId, [FromBody] JsonPatchDocument<ServiceInfoUpdatingDto> patchDocument, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var serviceInfoFromRepo = await _serviceInfoRepository.GetServiceInfo(serviceInfoId).ConfigureAwait(false);

            if (serviceInfoFromRepo == null) return NotFound();

            if (logginUserId != serviceInfoFromRepo.OwnerId && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            var serviceInfoToPatch = _mapper.Map<ServiceInfoUpdatingDto>(serviceInfoFromRepo);
            patchDocument.ApplyTo(serviceInfoToPatch, ModelState);

            if (!TryValidateModel(serviceInfoToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(serviceInfoToPatch, serviceInfoFromRepo); // Overriding

            await _serviceInfoRepository.UpdateServiceInfo(serviceInfoFromRepo).ConfigureAwait(false);

            await _serviceInfoRepository.Save().ConfigureAwait(false);

            var serviceInfoToReturn = _mapper.Map<ServiceInfoOutputDto>(serviceInfoFromRepo);

            return CreatedAtRoute(
                    "GetServiceInfo",
                    new { serviceInfoId = serviceInfoToReturn.Id, fields },
                    serviceInfoToReturn.ShapeData(fields)
                );
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeleteServiceInfo(Guid serviceInfoId, [FromQuery] string fields)
        {
            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var serviceInfoFromRepo = await _serviceInfoRepository.GetServiceInfo(serviceInfoId).ConfigureAwait(false);

            if (serviceInfoFromRepo == null) return NotFound();

            if (logginUserId != serviceInfoFromRepo.Owner.Id && (Role)Enum.Parse(typeof(Role), userRole) != Role.Admin) return Unauthorized();

            _serviceInfoRepository.DeleteServiceInfo(serviceInfoFromRepo);
            await _serviceInfoRepository.Save().ConfigureAwait(false);

            return Ok(_mapper.Map<ServiceInfoOutputDto>(serviceInfoFromRepo).ShapeData(fields));
        }


        private string CreateServiceInfosUri(ServiceInfoResourceParameters serviceInfoResourceParameters, UriType uriType, string routeName)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = serviceInfoResourceParameters.PageNumber - 1,
                        serviceInfoResourceParameters.PageSize,
                        serviceInfoResourceParameters.SearchQuery,
                        serviceInfoResourceParameters.OrderBy,
                        serviceInfoResourceParameters.Fields
                    });
                case UriType.NextPage:
                    return Url.Link(routeName, new
                    {
                        pageNumber = serviceInfoResourceParameters.PageNumber + 1,
                        serviceInfoResourceParameters.PageSize,
                        searchQuery = serviceInfoResourceParameters.SearchQuery,
                        serviceInfoResourceParameters.OrderBy,
                        serviceInfoResourceParameters.Fields
                    });
                default:
                    return Url.Link(routeName, new
                    {
                        serviceInfoResourceParameters.PageNumber,
                        serviceInfoResourceParameters.PageSize,
                        serviceInfoResourceParameters.SearchQuery,
                        serviceInfoResourceParameters.OrderBy,
                        serviceInfoResourceParameters.Fields
                    });

            }
        }
    }
}