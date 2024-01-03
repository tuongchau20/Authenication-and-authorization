using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Authorize.Services
{
    public class AuthenticationService
    {
        private readonly UserDbContext _context;
        private readonly TokenServices _tokenServices;
        private readonly ILoggerManager _logger;

        public AuthenticationService(UserDbContext context, TokenServices tokenServices, ILoggerManager logger)
        {
            _context = context;
            _tokenServices = tokenServices;
            _logger = logger;
        }

        public async Task<Response> Validate(LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && model.Password == model.Password);
            if (user == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "Invalid username/password"
                };
            }

            var token = await _tokenServices.GenerateToken(user);
            return new Response
            {
                Success = true,
                Message = "Authenticate success",
                Data = token
            };
        }
        public async Task<Response> Register(SignUpUser model)
        {
            // Kiểm tra xem username đã tồn tại chưa
            var userExists = _context.Users.Any(u => u.UserName == model.UserName);
            if (userExists)
            {
                return new Response
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            var newUser = new User
            {
                UserName = model.UserName,
                Password = model.Password, 
                Role = "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new Response
            {
                Success = true,
                Message = "User registered successfully"
            };
        }
    }
}
