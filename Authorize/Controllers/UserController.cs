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
        private readonly ILoggerManager _logger;
      
        public UserController(UserDbContext context, TokenServices tokenServices, UserService userService,ILoggerManager logger) 
        {
            _context = context;
            _userService = userService;
            _tokenServices = tokenServices;
            _logger = logger;
        }
        [HttpPost("Login")]
        public async Task <IActionResult> Validate (LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p=> p.UserName == model.UserName
            && model.Password == p.Password);
            if (user == null)
            {
                return Ok(new Response
                {
                    Success = false,
                    Message = "Invalid username/password"
                }) ;
            }
            //Gen token
            var token = await _tokenServices.GenerateToken(user);
            return Ok(new Response
            {
                Success = true,
                Message = "Authenticate success",
                Data = token
            });
        }
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var response = await _tokenServices.RenewToken(model);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
