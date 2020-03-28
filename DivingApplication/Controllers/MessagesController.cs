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
using DivingApplication.Models.Messages;
using DivingApplication.Repositories.ChatRooms;
using DivingApplication.Repositories.Messages;
using DivingApplication.Repositories.Users;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;
        public MessagesController(IMessageRepository messageRepository, IChatRepository chatRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation)
        {
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        public IPropertyValidationService PropertyValidation => _propertyValidation;

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("chatRoom/{chatRoomId}", Name = "GetChatRoomMessages")]
        public async Task<IActionResult> GetChatRoomMessages(Guid chatRoomId, [FromQuery] MessageResourceParameters resourceParameters)
        {

            if (!_propertyMapping.ValidMappingExist<MessageOutputDto, Message>(resourceParameters.OrderBy)) return BadRequest();
            if (!PropertyValidation.HasValidProperties<MessageOutputDto>(resourceParameters.Fields)) return BadRequest();


            // Checking if the user in the room or Admin

            var logginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Retrieving the ChatRoom

            var chatRoom = await _chatRepository.GetChatRoom(chatRoomId);

            List<Guid> memberIds = chatRoom.UserChatRooms.Select(ucr => ucr.UserId).ToList();
            if (!memberIds.Contains(logginUserId)) return BadRequest();

            resourceParameters.ChatRoomId = chatRoomId;

            var messagesFromRepo = await _messageRepository.GetMessages(resourceParameters);

            var previousPageLink = messagesFromRepo.HasPrevious ? CreatePostsUri(resourceParameters, UriType.PreviousPage, "GetChatRoomMessages") : null;
            var nextPageLink = messagesFromRepo.HasNext ? CreatePostsUri(resourceParameters, UriType.NextPage, "GetChatRoomMessages") : null;

            var metaData = new
            {
                totalCount = messagesFromRepo.TotalCount,
                pageSize = messagesFromRepo.PageSize,
                currentPage = messagesFromRepo.CurrentPage,
                totalPages = messagesFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };
            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<MessageOutputDto>>(messagesFromRepo).ShapeData(resourceParameters.Fields));
        }


        private string CreatePostsUri(MessageResourceParameters resourceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, resourceParameters.CreateUrlParameters(uriType));
        }
    }
}