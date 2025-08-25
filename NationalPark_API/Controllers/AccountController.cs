using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalPark_API.Models;
using NationalPark_API.Repository.IRepository;
using System.Net.Http.Headers;

namespace NationalPark_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository=userRepository;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if(ModelState.IsValid)
            {
                var isUniqueUser = _userRepository.IsUniqueUser(user.UserName);
                if (!isUniqueUser) return BadRequest("Already User in Use!!");
                var UserInfo =_userRepository.Register(user.UserName,user.Password);
                if (UserInfo == null) return NotFound();
                user = UserInfo; 
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var userFromDb = _userRepository.Authenticate(user.UserName, user.Password);
            if (userFromDb == null) return BadRequest("Wrong User/Password");
            return Ok(userFromDb);
        }

    }
}
       