using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Authorize.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Authorize.Model.UserConstants;

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
        private readonly AuthenticationService _authenticationService;

        public UserController(UserDbContext context, TokenServices tokenServices, UserService userService,ILoggerManager logger, AuthenticationService authenticationService)
        {
            _context = context;
            _userService = userService;
            _tokenServices = tokenServices;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var response = await _tokenServices.RenewToken(model);
            return response.Success ? Ok(response) : BadRequest(response);
        }
       
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            var rs = await _authenticationService.Validate(model);
            return Ok(rs);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(SignUpUser model)
        {
            var response = await _authenticationService.Register(model);
            return Ok(response);
        }
    }
}
