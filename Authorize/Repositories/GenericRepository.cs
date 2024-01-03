using Authorize.Data;
using Authorize.Model;

namespace Authorize.Repositories
{
    public class GenericRepository
    {
        private readonly UserDbContext _context;

        public GenericRepository(UserDbContext context)
        {
            _context = context;
        }

        public User GetUser(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(Guid id)
        {
            var user = GetUser(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

    }
}
