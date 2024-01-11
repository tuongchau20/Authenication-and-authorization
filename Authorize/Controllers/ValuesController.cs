using Authorize.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Authorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ValuesController(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = cache;
            _connectionMultiplexer = connectionMultiplexer;
        }
        [HttpPost("cache-test")]
        public async Task<IActionResult> CacheTest([FromBody] CacheItem cacheItem)
        {
            await _distributedCache.SetStringAsync(cacheItem.Key, cacheItem.Data);
            var cachedData = await _distributedCache.GetStringAsync(cacheItem.Key);

            return Ok(cachedData);
        }


        [HttpDelete("delete-keys")]
        public IActionResult DeleteKeysWithPrefix(string prefix)
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            var server = _connectionMultiplexer.GetServer(endpoints.First()); 
            var keys = server.Keys(pattern: $"{prefix}*");
            var db = _connectionMultiplexer.GetDatabase();
            foreach (var key in keys)
            {
                db.KeyDelete(key);
            }

            return Ok("Keys deleted.");
        }
        [HttpGet("get-all-keys")]
        public IActionResult GetAllKeys()
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys();
            return Ok(keys.Select(key => key.ToString()));
        }

    }
}
