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
using DivingApplication.Models.ChatRoom;
using DivingApplication.Repositories.ChatRooms;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;
        public ChatRoomsController(IChatRepository chatRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("self", Name = "GetSelfChatRooms")]
        public async Task<IActionResult> GetSelfChatRooms([FromQuery] ChatRoomResourceParameters resourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<ChatRoomOutputDto, ChatRoom>(resourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<ChatRoomOutputDto>(resourceParameters.Fields)) return BadRequest();


            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            resourceParameters.MemberId = logginUserId;

            var chatRoomsFromRepo = await _chatRepository.GetSelfChatRooms(resourceParameters);

            var previousPageLink = chatRoomsFromRepo.HasPrevious ? CreatePostsUri(resourceParameters, UriType.PreviousPage, "GetSelfChatRooms") : null;
            var nextPageLink = chatRoomsFromRepo.HasNext ? CreatePostsUri(resourceParameters, UriType.NextPage, "GetSelfChatRooms") : null;

            var metaData = new
            {
                totalCount = chatRoomsFromRepo.TotalCount,
                pageSize = chatRoomsFromRepo.PageSize,
                currentPage = chatRoomsFromRepo.CurrentPage,
                totalPages = chatRoomsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<ChatRoomOutputDto>>(chatRoomsFromRepo).ShapeData(resourceParameters.Fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet(Name = "GetChatRooms")]
        public async Task<IActionResult> GetChatRooms([FromQuery] ChatRoomResourceParameters resourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<ChatRoomOutputDto, ChatRoom>(resourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<ChatRoomOutputDto>(resourceParameters.Fields)) return BadRequest();

            var chatRoomsFromRepo = await _chatRepository.GetChatRooms(resourceParameters);

            var previousPageLink = chatRoomsFromRepo.HasPrevious ? CreatePostsUri(resourceParameters, UriType.PreviousPage, "GetChatRooms") : null;
            var nextPageLink = chatRoomsFromRepo.HasNext ? CreatePostsUri(resourceParameters, UriType.NextPage, "GetChatRooms") : null;

            var metaData = new
            {
                totalCount = chatRoomsFromRepo.TotalCount,
                pageSize = chatRoomsFromRepo.PageSize,
                currentPage = chatRoomsFromRepo.CurrentPage,
                totalPages = chatRoomsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<ChatRoomOutputDto>>(chatRoomsFromRepo).ShapeData(resourceParameters.Fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("{chatRoomId}", Name = "GetChatRoom")]
        public async Task<IActionResult> GetChatRoom(Guid chatRoomId, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<ChatRoomOutputDto>(fields)) return BadRequest();

            var chatRoomFromRepo = await _chatRepository.GetChatRoom(chatRoomId);

            if (chatRoomFromRepo == null) return NotFound();

            return Ok(_mapper.Map<ChatRoomOutputDto>(chatRoomFromRepo).ShapeData(fields));
        }



        private string CreatePostsUri(ChatRoomResourceParameters resourceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, resourceParameters.CreateUrlParameters(uriType));
        }
    }
}