using Microsoft.AspNetCore.Mvc;
using TestUserProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet]
        [Route("getallusers")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userRepository.GetAllUsers());
        }

        [HttpPost]
        [Route("store")]
        public async Task<IActionResult> Store([FromBody] string user)
        {
            var userForInsert = new User { Name = user };
            var retv = await _userRepository.CreateAsync(userForInsert);
            _rmqCommand.SendMessage(user);
            return Ok(retv);
        }
    }
}
