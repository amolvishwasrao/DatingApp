using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Data;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;

        public readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config )
        {
            _repo=repo;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserForRegsiterDto userForResgiterDto)
        {
           
             userForResgiterDto.Username= userForResgiterDto.Username.ToLower();
             if(await _repo.UserExist(userForResgiterDto.Username) )
             {
                 return BadRequest("User is already exits");
             }
             var userToCreate = new User{
                 UserName=userForResgiterDto.Username
             };
             var creatdUser=await _repo.Register(userToCreate,userForResgiterDto.Password);
             return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForloginDto)
        {
            var userromRepo = await _repo.Login(userForloginDto.Username.ToLower
                (), userForloginDto.Password);
            if (userromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userromRepo.UserName.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token =  tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token=tokenHandler.WriteToken(token)});
                



        }

    }
}