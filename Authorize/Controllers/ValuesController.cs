using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Authorize.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        public ValuesController(IDistributedCache cache)
        {
            _cache = cache;
        }
        [HttpGet("cache-test")]
        public async Task<IActionResult> CacheTest()
        {
            var cacheKey = "testKey2";
            var testData = "Hello, Redis!2";

            await _cache.SetStringAsync(cacheKey, testData);

            var cachedData = await _cache.GetStringAsync(cacheKey);

            return Ok(cachedData);
        }


    }
}
