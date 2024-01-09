using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Authorize.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Authorize.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly UserDbContext _context;
        private readonly TokenServices _tokenServices;

        public UserService(UserRepository genericRepository, UserDbContext context,TokenServices tokenServices)
        {
            _userRepository = genericRepository;
            _context = context;
            _tokenServices=tokenServices;
        }

        public User GetUser(Guid id)
        {
            return _userRepository.GetUser(id);
        }
        public IEnumerable<User> GetUsersByRole(string roleName)
        {
            return _context.Users.Where(u => u.Role == roleName).ToList();
        }
        public void CreateUser(User user)
        {
            _userRepository.AddUser(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(string userName)
        {
            _userRepository.DeleteUser(userName);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
        
    }
}