using Authorize.Data;
using Authorize.Model;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Authorize.Repositories
{
    public class UserRepository
    {
        private readonly UserDbContext _context;
        private readonly IDistributedCache _cache;

        public UserRepository(UserDbContext context , IDistributedCache cache)
        {
            _context = context;
            _cache = cache;

        }

        public User GetUser(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var cacheKey = "allUsers";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                Console.WriteLine("Retrieved data from cache.");
                return JsonConvert.DeserializeObject<IEnumerable<User>>(cachedData);
            }

            var users = await _context.Users.ToListAsync();
            if (users != null && users.Any())
            {
                var serializedUsers = JsonConvert.SerializeObject(users);
                Console.WriteLine("Serialized Users: " + serializedUsers); // Logging the serialized data

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                };

                await _cache.SetStringAsync(cacheKey, serializedUsers, cacheEntryOptions);
                Console.WriteLine("Data stored in cache.");

                return users;
            }
            else
            {
                Console.WriteLine("No users found in the database.");
                return new List<User>();
            }
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

        public void DeleteUser(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public IEnumerable<User> GetUsersByRole()
        {
            return _context.Users.Where(u => u.Role == "User").ToList();
        }

    }
}
