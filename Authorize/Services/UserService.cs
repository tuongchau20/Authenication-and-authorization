using Authorize.Data;
using Authorize.Helper;
using Authorize.Model;
using Authorize.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Authorize.Services
{
    public class UserService
    {
        private readonly UserRepository _genericRepository;
        private readonly UserDbContext _context;
        private readonly TokenServices _tokenServices;

        public UserService(UserRepository genericRepository, UserDbContext context,TokenServices tokenServices)
        {
            _genericRepository = genericRepository;
            _context = context;
            _tokenServices=tokenServices;
        }

        public User GetUser(Guid id)
        {
            return _genericRepository.GetUser(id);
        }

        public void CreateUser(User user)
        {
            _genericRepository.AddUser(user);
        }

        public void UpdateUser(User user)
        {
            _genericRepository.UpdateUser(user);
        }

        public void DeleteUser(Guid id)
        {
            _genericRepository.DeleteUser(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _genericRepository.GetAllUsers();
        }
     
    }
}