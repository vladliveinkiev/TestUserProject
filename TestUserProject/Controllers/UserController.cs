using Microsoft.AspNetCore.Mvc;
using TestUserProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace TestUserProject.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRabbitMqCommand _rmqCommand;

        public UserController(IRabbitMqCommand rmqCommand, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _rmqCommand = rmqCommand;
        }

        [SwaggerOperation(Summary = "Get all users from database")]
        [SwaggerResponse(statusCode: 200, Description = "Return token for api authorization")]
        [SwaggerResponse(statusCode: 404, Description = "Users not found in the database")]
        [HttpGet]
        [Route("getallusers")]
        public IActionResult GetAllUsers()
        {

            var allUsers = _userRepository.GetAllUsers();

            if (allUsers != null)
                return Ok(_userRepository.GetAllUsers());
            else
                return NotFound("No users in database");
        }

        [SwaggerOperation(Summary = "Send a command to add new user into RabbitMq queue")]
        [SwaggerResponse(statusCode: 200, Description = "Command to add user was sent!")]
        [HttpPost]
        [Route("store")]
        public IActionResult Store([FromBody] string user)
        {
            var userForInsert = new User { Name = user };
            //var retv = await _userRepository.CreateAsync(userForInsert);
            _rmqCommand.SendMessage(user);
            return Ok("Command to add user was sent!");
        }

        [SwaggerOperation(Summary = "Create new user in database and send Command into RabbitMq queue")]
        [SwaggerResponse(statusCode: 200, Description = "Return Guid of the created user")]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] string user)
        {
            var userForInsert = new User { Name = user };
            var retv = await _userRepository.CreateAsync(userForInsert);
            return Ok(retv.ToString());
        }
    }
}
