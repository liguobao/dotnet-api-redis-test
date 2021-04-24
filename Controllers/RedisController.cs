using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_api_redis_test.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_api_redis_test.Controllers
{
    [ApiController]
    public class RedisController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly RedisService _redisService;

        public RedisController(ILogger<WeatherForecastController> logger, RedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }

        [HttpGet("redis-lock-v1")]
        public async Task<IActionResult> GetV1()
        {
            var lockKey = new Guid().ToString();
            var lockValue = new Guid().ToString();
            var takeResult = await _redisService.LockTakeAsync(lockKey, lockValue);
            var releaseResult = await _redisService.LockReleaseByValueAsync(lockKey, lockValue);
            return Ok(new { takeResult = takeResult, releaseResult = releaseResult });
        }


        [HttpGet("redis-lock-v2")]
        public async Task<IActionResult> GetV2()
        {
            var lockKey = new Guid().ToString();
            var lockValue = new Guid().ToString();
            var takeResult = await _redisService.LockTakeAsync(lockKey, lockValue);
            var releaseResult = await _redisService.LockReleaseAsync(lockKey, lockValue);
            return Ok(new { takeResult = takeResult, releaseResult = releaseResult });
        }
    }
}
