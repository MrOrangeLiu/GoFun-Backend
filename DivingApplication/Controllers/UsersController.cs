using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Repositories;
using DivingApplication.Models;
using AutoMapper;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettingsService _appSettings;


        public UsersController(IUserRepository userRepository, IMapper mapper, IOptions<AppSettingsService> appSettings)
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

        [Authorize(Policy = "NormalAndAdmin")]
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

        [Authorize(Policy = "NormalAndAdmin")]
        [HttpGet("reach/policy/normalAndAdmin")]
        public IActionResult NormalAndAdminReachTest()
        {
            return Ok(
                new
                {
                    msg = "Reach NormalAndAdminPolicy"
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

    }
}
