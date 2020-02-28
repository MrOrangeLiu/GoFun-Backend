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

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettingsService _appSettings;


        public UsersController(IUserRepository userRepository,
                               IMapper mapper,
                               IOptions<AppSettingsService> appSettings,
                               IPropertyMappingService propertyMapping,
                               IPropertyValidationService propertyValidation)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
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
        [HttpGet("follow/followers")]
        public async Task<IActionResult> GetAllFollowers([FromQuery]Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var allFollowers = await _userRepository.GetAllFollowers(userId);

            var allFollwersToReturn = _mapper.Map<IEnumerable<UserBriefOutputDto>>(allFollowers);

            return Ok(allFollwersToReturn);
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("follow/following")]
        public async Task<IActionResult> GetAllFollowing([FromQuery]Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var allFollowing = await _userRepository.GetAllFollowing(userId);

            var allFollowingToReturn = _mapper.Map<IEnumerable<UserBriefOutputDto>>(allFollowing);

            return Ok(allFollowingToReturn);
        }

        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts/save")]
        public async Task<IActionResult> GetAllSavePosts([FromQuery]Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userSavePosts = await _userRepository.GetAllSavePosts(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userSavePosts));
        }



        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts/like")]
        public async Task<IActionResult> GetAllLikePosts([FromQuery]Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userLikePosts = await _userRepository.GetAllLikePosts(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userLikePosts));
        }




        [Authorize(Policy = "VerifiedUsers")]
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllOwningPosts([FromQuery]Guid userId)
        {
            if (userId == Guid.Empty)
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var userOwningPosts = await _userRepository.GetAllOwningPost(userId);

            return Ok(_mapper.Map<IEnumerable<PostOutputDto>>(userOwningPosts));
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
            _userRepository.UpdateUser(userFromRepo);
            _userRepository.Save();

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

    }
}
