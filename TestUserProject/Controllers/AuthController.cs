using Microsoft.AspNetCore.Mvc;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TestUserProject.Models;
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace TestUserProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<MongoUser> _userManager; 
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<MongoUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [SwaggerOperation(Summary = "Login into API with existing user")]
        [SwaggerResponse(statusCode:200, Description ="Return token for api authorization")]
        [SwaggerResponse(statusCode: 404, Description = "User not found in the database")]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // generation of JWT token
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };

                var expireInTime = DateTime.Now.AddDays(30);
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    expires: expireInTime,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );


                return Ok(new  
                {
                    Status = "Success",
                    Message = "Successfully logged in!",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = expireInTime
                });
            }
            else
                return BadRequest("Token was not created. Try later...");
        }

        [SwaggerOperation(Summary = "Cretes user for further login")]
        [SwaggerResponse(statusCode: 200, Description = "User created successfully!")]
        [SwaggerResponse(statusCode: 409, Description = "User already exists in the database")]
        [SwaggerResponse(statusCode: 500, Description = "User not created due to internal errors")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] LoginUser model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            if (user != null)
            {
                return StatusCode(StatusCodes.Status409Conflict,"User already exists!");
            }
            MongoUser userToCreate = new MongoUser { UserName = model.Name };
            var result = await _userManager.CreateAsync(userToCreate, model.Password);
            if (result.Succeeded)
                return Ok("User created successfully!");
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "User not created!");
        }

    }
}
