using Authorize.Model;
using Authorize.Repositories;

namespace Authorize.Services
{
    public class UserService
    {
        private readonly GenericRepository _genericRepository;

        public UserService(GenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
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