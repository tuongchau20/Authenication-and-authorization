using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Authorize.Services
{
    public class AuthenticationService
    {
        private readonly UserDbContext _context;
        private readonly TokenServices _tokenServices;
        private readonly PasswordHasher<User> _passwordHasher;
        public AuthenticationService(UserDbContext context, TokenServices tokenServices)
        {
            _context = context;
            _tokenServices = tokenServices;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<Response> Validate(LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == model.UserName);

            if (user != null && _passwordHasher.VerifyHashedPassword(null, user.Password, model.Password) == PasswordVerificationResult.Success)
            {
                var token = await _tokenServices.GenerateToken(user);
                return new Response { 
                    Success = true, 
                    Message = "Authenticate success",
                    Data = token 
                };
            }

            return new Response { 
                Success = false, 
                Message = "Invalid username/password" 
            };
        }

        public async Task<Response> Register(SignUpUser model)
        {
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
                Password = _passwordHasher.HashPassword(null, model.Password),
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
