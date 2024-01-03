using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Authorize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly UserService _userService;
        private readonly TokenServices _tokenServices;

        public UserController(UserDbContext context, TokenServices tokenServices, UserService userService) 
        {
            _context = context;
            _userService = userService;
            _tokenServices = tokenServices;
        }
        [HttpPost("Login")]
        public IActionResult Validate (LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p=> p.UserName == model.UserName
            && model.Password == p.Password);
            if (user == null)
            {
                return Ok(new Response
                {
                    Succsess = false,
                    Message = "Invalid username/password"
                }) ;
            }
            //cấp token
            return Ok(new Response
            {
                Succsess = true,
                Message = "Authenticate success",
                Data = _tokenServices.GenerateToken(user)
            });
        }
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
    }
}
