using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DivingApplication.Entities;
using DivingApplication.Repositories.Users;
using AutoMapper;
using DivingApplication.Services;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using DivingApplication.Helpers;
using DivingApplication.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using DivingApplication.Models.Posts;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using static DivingApplication.Entities.User;
using DivingApplication.Services.PropertyServices;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Models.CoachInfo;
using DivingApplication.Models.ServiceInfo;
using DivingApplication.Helpers.ResourceParameters;
using System.Text.Json;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettingsService _appSettings;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;



        public UsersController(IUserRepository userRepository,
                               IMapper mapper,
                               IOptions<AppSettingsService> appSettings,
                               IPropertyMappingService propertyMapping,
                               IPropertyValidationService propertyValidation)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _propertyMapping = propertyMapping;
            _propertyValidation = propertyValidation;
        }

        // Success : http://localhost:65000/api/users/test

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Testing()
        {
            return Ok(new
            {
                title = "Hello World!",
            });
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SingupUser([FromBody] UserForCreatingDto userForCreatingDto)
        {
            if (await _userRepository.UserEmailExists(userForCreatingDto.Email)) return BadRequest("User Already Exist");

            var user = _mapper.Map<User>(userForCreatingDto);

            await _userRepository.AddUser(user, userForCreatingDto.Password);
            await _userRepository.Save();

            // Generating JWT Token
            var token = GenerateTokenForUser(user);

            var userToReturn = _mapper.Map<UserOutputDto>(user);

            if (token == null) return BadRequest();

            return Ok(new
            {
                token,
                user = userToReturn,
            }); // Return UserOutputDto and Token
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginInfo loginInfo)
        {
            var user = _userRepository.Authenticate(loginInfo.Email, loginInfo.Password);

            if (user == null) return NotFound();

            var token = GenerateTokenForUser(user);

            if (token == null) return BadRequest();

            // Updating LastSeen
            user.LastSeen = DateTime.Now;
            await _userRepository.UpdateUser(user);
            await _userRepository.Save();

            var userToReturn = _mapper.Map<UserOutputDto>(user);

            return Ok(
                new
                {
                    user = userToReturn,
                    token,
                });
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPatch("{userId}")]
        public async Task<ActionResult> PartiallyUpdateUser(Guid userId, [FromBody] JsonPatchDocument<UserUpdatingDto> patchDocument)
        {
            var logginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the updating user and the current user is the same one.
            if (Guid.Parse(logginUserId) != userId) return BadRequest();

            var userFromRepo = await _userRepository.GetUser(userId);

            if (userFromRepo == null) return NotFound();

            var userToPatch = _mapper.Map<UserUpdatingDto>(userFromRepo);

            patchDocument.ApplyTo(userToPatch, ModelState);

            if (!TryValidateModel(userToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(userToPatch, userFromRepo); // Overriding 
            userFromRepo.LastSeen = DateTime.Now;
            await _userRepository.UpdateUser(userFromRepo); //Faking
            await _userRepository.Save();

            var userToReturn = _mapper.Map<UserOutputDto>(userFromRepo);

            return CreatedAtRoute(
                    "GetUserById",
                    new
                    {
                        userId = userToReturn.Id,
                    },
                    userToReturn
                );
        }

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetUserById")]
        public async Task<ActionResult<UserOutputDto>> GetUser(Guid userId)
        {
            var userFromRepo = await _userRepository.GetUser(userId);
            if (userFromRepo == null) return NotFound();

            return Ok(_mapper.Map<UserOutputDto>(userFromRepo));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost("follow/{followingUserId}")]
        public async Task<IActionResult> UserFollowToggle(Guid followingUserId)
        {
            var followerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (followerUserId == followingUserId) return BadRequest("Cannot follow yourself");

            // Checking if the user Exist

            var followingUser = await _userRepository.GetUser(followerUserId);

            if (followingUser == null) return NotFound();

            var currentFollowStatus = await _userRepository.GetCurrentUserFollow(followerUserId, followingUserId);
            bool Adding;

            if (currentFollowStatus == null)
            {
                currentFollowStatus = await _userRepository.UserFollowUser(followerUserId, followingUserId);
                await _userRepository.Save();
                Adding = true;

            }
            else
            {
                _userRepository.UserUnFollowUser(currentFollowStatus);
                await _userRepository.Save();
                Adding = false;

            }
            return Ok(
                new
                {
                    Adding,
                    currentFollowStatus.FollowerId,
                    currentFollowStatus.FollowingId,
                }
                );
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("follow/followers/{userId}")]
        public async Task<IActionResult> GetAllFollowers(Guid userId, [FromQuery]string fields)
        {
            if (!_propertyValidation.HasValidProperties<UserBriefOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var allFollowers = await _userRepository.GetAllFollowers(userId);

            var allFollwersToReturn = _mapper.Map<IEnumerable<UserBriefOutputDto>>(allFollowers);

            return Ok(allFollwersToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("follow/following/{userId}")]
        public async Task<IActionResult> GetAllFollowing(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<UserBriefOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }


            var allFollowing = await _userRepository.GetAllFollowing(userId);

            var allFollowingToReturn = _mapper.Map<IEnumerable<UserBriefOutputDto>>(allFollowing);

            return Ok(allFollowingToReturn.ShapeData(fields));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts/save/{userId}")]
        public async Task<IActionResult> GetAllSavePosts(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userSavePosts = await _userRepository.GetAllSavePosts(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userSavePosts.ShapeData(fields)));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers([FromQuery] UserResourceParameterts userResourceParameterts)
        {
            if (!_propertyMapping.ValidMappingExist<UserBriefOutputDto, User>(userResourceParameterts.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<UserBriefOutputDto>(userResourceParameterts.Fields)) return BadRequest();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var usersFromRepo = _userRepository.GetUsers(userResourceParameterts, userId);

            var previousPageLink = usersFromRepo.HasPrevious ? CreatePostsUri(userResourceParameterts, UriType.PreviousPage, "GetUsers") : null;
            var nextPageLink = usersFromRepo.HasNext ? CreatePostsUri(userResourceParameterts, UriType.NextPage, "GetUsers") : null;

            var metaData = new
            {
                totalCount = usersFromRepo.TotalCount,
                pageSize = usersFromRepo.PageSize,
                currentPage = usersFromRepo.CurrentPage,
                totalPages = usersFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<UserBriefOutputDto>>(usersFromRepo).ShapeData(userResourceParameterts.Fields));
        }




        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts/like/{userId}")]
        public async Task<IActionResult> GetAllLikePosts(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userLikePosts = await _userRepository.GetAllLikePosts(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userLikePosts.ShapeData(fields)));
        }




        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts/{userId}")]
        public async Task<IActionResult> GetAllOwningPosts(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<PostOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userOwningPosts = await _userRepository.GetAllOwningPost(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userOwningPosts.ShapeData(fields)));
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("coachInfo/{userId}")]
        public async Task<IActionResult> GetCoachInfoForUser(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<CoachInfoOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }


            var userCoachInfo = await _userRepository.GetCoachInfoForUser(userId);

            return Ok(_mapper.Map<IEnumerable<CoachInfoOutputDto>>(userCoachInfo.ShapeData(fields)));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("serviceInfo/{userId}")]
        public async Task<IActionResult> GetServiceInfoForUser(Guid userId, [FromQuery] string fields)
        {
            if (!_propertyValidation.HasValidProperties<ServiceInfoOutputDto>(fields)) return BadRequest();

            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userServiceInfo = await _userRepository.GetServiceInfoForUser(userId);

            return Ok(_mapper.Map<IEnumerable<ServiceInfoOutputDto>>(userServiceInfo.ShapeData(fields)));
        }



        [Authorize(Roles = "EmailNotVerified")]
        [HttpPost("email/send")]
        public async Task<IActionResult> SendingEmailToUserTest()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userFromRepo = await _userRepository.GetUser(userId);

            if (userFromRepo == null) return NotFound();

            if (userFromRepo.UserRole != Role.EmailNotVerified) return BadRequest();

            // Generate the Verification

            var code = GenerateVerificationString(6);

            // Saving it to Db
            userFromRepo.EmailVerificationCode = code;
            userFromRepo.LastSeen = DateTime.Now;
            await _userRepository.UpdateUser(userFromRepo);
            await _userRepository.Save();

            var sendingMessage = new MimeMessage
            {
                Sender = new MailboxAddress("DivingApp", "diving_app_2020@outlook.com"),
                Subject = "Diving App 驗證信",
            };


            sendingMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = $"</br></br><center><h1> 使用者 { userFromRepo.Name } 您好 </h1></center></br><center><h3> 您的身分驗證碼為: {code} </ h3></center></br></br><center><p> Diving App 團隊 </p><p> 敬上<p> </center> "
            };

            sendingMessage.To.Add(new MailboxAddress(userFromRepo.Email));


            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.MessageSent += (sender, args) => { };

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_appSettings.Email, _appSettings.EmailPassword);

                await smtp.SendAsync(sendingMessage);

                await smtp.DisconnectAsync(true);

            }

            return Ok(new
            {
                userFromRepo.Email,
                userFromRepo.Name,
            });
        }

        [Authorize(Roles = "EmailNotVerified")]
        [HttpPost("email/verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerifyInfo emailVerifyInfo)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userFromRepo = await _userRepository.GetUser(userId);

            if (userFromRepo == null) return NotFound();

            if (userFromRepo.UserRole != Role.EmailNotVerified) return BadRequest();

            if (string.IsNullOrWhiteSpace(emailVerifyInfo.Code)) return BadRequest();

            if (userFromRepo.EmailVerificationCode.ToLower() != emailVerifyInfo.Code.ToLower()) return BadRequest();

            userFromRepo.UserRole = Role.Normal;

            _userRepository.Save();

            var userToReturn = _mapper.Map<UserOutputDto>(userFromRepo);

            return Ok(userToReturn);

        }








        //[AllowAnonymous]
        //[HttpDelete("follow")]
        //public async Task<IActionResult> DeleteAllUserFollow()
        //{
        //   await _userRepository.RemoveAllUserFollow();
        //    return Ok();
        //}



        [AllowAnonymous]
        [HttpGet("reach/notLogin")]
        public IActionResult NotLoginReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach not login"
                }
                );
        }

        [Authorize(Roles = "Normal")]
        [HttpGet("reach/normal")]
        public IActionResult NormalReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach Normal"
                }
                );
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("reach/normal")]
        public IActionResult CoachReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach Coach"
                }
                );
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("reach/admin")]
        public IActionResult AdminReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach Admin"
                }
                );
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("reach/policy/VerifiedUsers")]
        public IActionResult VerifiedUsersReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach VerifiedUsersPolicy"
                }
                );
        }


        private string GenerateTokenForUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretForJwt);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                new Claim(
                    ClaimTypes.NameIdentifier,user.Id.ToString()
                    ),
                new Claim(
                    ClaimTypes.Name, user.Name
                    ),
                new Claim(
                    ClaimTypes.Email, user.Email
                    ),
                new Claim(
                    ClaimTypes.Role, user.UserRole.ToString()
                    ),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerateVerificationString(int length)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";


            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                builder.Append(characters[random.Next(characters.Length)]);
            }

            return builder.ToString();
        }

        private string CreatePostsUri(UserResourceParameterts userResrouceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, userResrouceParameters.CreateUrlParameters(uriType));
        }

    }
}
