using Authorize.Data;
using Authorize.Model;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Authorize.Repositories
{
    public class UserRepository
    {
        private readonly UserDbContext _context;
        private readonly IAppCache _lazyCache;
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redisConnection;

        public UserRepository(UserDbContext context, IAppCache lazyCache, IDistributedCache distributedCache, IConnectionMultiplexer redisConnection)
        {
            _context = context;
            _lazyCache = lazyCache;
            _distributedCache = distributedCache;
            _redisConnection = redisConnection;
        }


        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var cacheKey = "allUsers";
            try
            {
                //  check cache LazyCache
                var cachedData = await _lazyCache.GetOrAddAsync(cacheKey, async () =>
                {
                    try
                    {
                        // check cache Redis
                        var redisDb = _redisConnection.GetDatabase();
                        var redisData = await redisDb.StringGetAsync(cacheKey);

                        if (redisData.HasValue)
                        {
                            // Nếu có trong Redis, save vào LazyCache để lần sau truy xuất nhanh hơn
                            var cachedUsers = JsonConvert.DeserializeObject<IEnumerable<User>>(redisData);
                            return cachedUsers;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error when retrieving from Redis: " + ex.Message);
                    }

                    try
                    {
                        // Nếu không có trong cache, truy vấn cơ sở dữ liệu
                        var users = await _context.Users.ToListAsync();
                        var serializedUsers = JsonConvert.SerializeObject(users);

                        var cacheEntryOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                        };

                        // save cả  Redis, LazyCache
                        var redisDb = _redisConnection.GetDatabase();
                        await redisDb.StringSetAsync(cacheKey, serializedUsers, TimeSpan.FromMinutes(60));

                        return users;
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error when retrieving from database or setting cache: " + ex.Message);
                        throw;
                    }
                });

                return cachedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when retrieving from cache or database: " + ex.Message);
                throw;
            }
        }
        public User GetUser(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
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

