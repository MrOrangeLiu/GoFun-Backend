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

        [HttpPost("signup")]
        public async Task<IActionResult> SingupUser([FromBody] UserForCreatingDto userForCreatingDto)
        {
            var user = _mapper.Map<User>(userForCreatingDto);

            await _userRepository.AddUser(user, userForCreatingDto.Password);

            // Generating JWT Token
            var token = GenerateTokenForUser(user);

            if (token == null) return BadRequest();

            return Ok(new
            {
                token,
                user,
            }); // Return UserOutputDto and Token
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginInfo loginInfo)
        {
            var user = _userRepository.Authenticate(loginInfo.Email, loginInfo.Password);

            if (user == null) return NotFound();

            var token = GenerateTokenForUser(user);

            if (token == null) return BadRequest();

            return Ok(new
            {
                user,
                token,
            });
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
                    ClaimTypes.Name,user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

    }
}
