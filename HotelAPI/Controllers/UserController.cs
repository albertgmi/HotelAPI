using HotelAPI.Models.UserModels;
using HotelAPI.Services.UserServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [Route("/api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto userDto)
        {
            _userService.RegisterUser(userDto);
            return Ok();
        }
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto loginDto)
        {
            var token = _userService.GenerateJwt(loginDto);
            return Ok(token);
        }
        [HttpPost("{userId}/make-admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult MakeAdmin([FromRoute] int userId)
        {
            _userService.MakeAdmin(userId);
            return Ok($"User with id: {userId} is now an admin!");
        }
        [HttpPost("{userId}/make-manager")]
        [Authorize(Roles = "Admin")]
        public ActionResult MakeManager([FromRoute] int userId)
        {
            _userService.MakeManager(userId);
            return Ok($"User with id: {userId} is now a manager!");
        }
    }
}
