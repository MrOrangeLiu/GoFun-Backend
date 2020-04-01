using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.ChatRooms;
using DivingApplication.Repositories.ChatRooms;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomsController(IChatRepository chatRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation, IHubContext<ChatHub> hubContext)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }


        [Authorize(Policy = "ExceptBanned")]
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


        [Authorize(Policy = "ExceptBanned")]
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


        [Authorize(Policy = "ExceptBanned")]
        [HttpGet("{chatRoomId}", Name = "GetChatRoom")]
        public async Task<IActionResult> GetChatRoom(Guid chatRoomId, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<ChatRoomOutputDto>(fields)) return BadRequest();

            var chatRoomFromRepo = await _chatRepository.GetChatRoom(chatRoomId);

            if (chatRoomFromRepo == null) return NotFound();

            return Ok(_mapper.Map<ChatRoomOutputDto>(chatRoomFromRepo).ShapeData(fields));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("{anotherUserId}", Name = "CreatePrivateChatRoom")]
        public async Task<IActionResult> CreatePrivateChatRoom(Guid anotherUserId)
        {

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            // Create ChatRoom

            var newChatRoom = new ChatRoom()
            {
                IsGroup = false,
            };

            await _chatRepository.AddChatRoom(newChatRoom);

            // Add two members in 

            newChatRoom.UserChatRooms.AddRange(
                new List<UserChatRoom> {
                    new UserChatRoom()
                        {
                            ChatRoomId = newChatRoom.Id,
                            UserId = userId,
                        },
                    new UserChatRoom()
                        {
                            ChatRoomId = newChatRoom.Id,
                            UserId = anotherUserId,
                        },
                }
            );

            await _chatRepository.Save();

            var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(newChatRoom);

            return Ok(chatRoomToReturn);
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("group", Name = "CreateGroupChatRoom")]
        //ChatRoomForCreatingDto chatRoom
        public async Task<IActionResult> CreateGroupChatRoom([FromBody] ChatRoomForCreatingDto chatRoom)
        {
            // Get CurrentUser Id
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Transform to Entity
            var chatRoomEntity = _mapper.Map<ChatRoom>(chatRoom);

            // Adding this ChatRoom to Db for Id
            await _chatRepository.AddChatRoom(chatRoomEntity);

            // Modify the Required Fields
            chatRoomEntity.IsGroup = true;
            chatRoomEntity.CreatedAt = DateTime.Now;
            chatRoomEntity.UserChatRooms.Add(
                    new UserChatRoom()
                    {
                        ChatRoomId = chatRoomEntity.Id,
                        UserId = userId,
                        Role = UserChatRoom.UserChatRoomRole.Owner,
                    }
                );

            // Save to Db
            await _chatRepository.Save();

            // Tranform to ReturnType
            var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(chatRoomEntity);

            // Return the ChatRoom
            return Ok(chatRoomToReturn);
        }


        private string CreatePostsUri(ChatRoomResourceParameters resourceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, resourceParameters.CreateUrlParameters(uriType));
        }
    }
}