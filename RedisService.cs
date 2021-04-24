using dotnet_api_redis_test.AOP;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_redis_test.Service
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redisMultiplexer;

        private readonly ILogger _logger;


        public RedisService(ConnectionMultiplexer redisMultiplexer, ILogger<RedisService> logger)
        {
            _redisMultiplexer = redisMultiplexer;
            //_redisMultiplexer.PreserveAsyncOrder = true;
            _logger = logger;
        }

        [TimeAspect(typeof(RedisService))]
        public virtual async Task<bool> LockTakeAsync(string lockKey, object value, int lockSeconds = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(lockKey))
                {
                    return false;
                }

                string cacheValue = ConvertToCacheValue(value);
                IDatabase db = _redisMultiplexer.GetDatabase(0);
                return await db.LockTakeAsync(lockKey, cacheValue, TimeSpan.FromSeconds(lockSeconds));
            }
            catch (Exception ex)
            {
                _logger.LogError($"LockTakeAsync fail,key:{lockKey},ex.Message:{ex.Message}, StackTrace:{ex.StackTrace}");
                return false;
            }
        }

        [TimeAspect(typeof(RedisService))]
        public virtual async Task<bool> LockReleaseByValueAsync(string lockKey, object value)
        {
            try
            {
                if (string.IsNullOrEmpty(lockKey))
                {
                    return false;
                }

                string cacheValue = ConvertToCacheValue(value);
                IDatabase db = _redisMultiplexer.GetDatabase(0);
                return await db.LockReleaseAsync(lockKey, cacheValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LockReleaseByValueAsync fail,key:{lockKey},ex.Message:{ex.Message}, StackTrace:{ex.StackTrace}");
                return false;
            }
        }

        [TimeAspect(typeof(RedisService))]
        public virtual async Task<bool> LockReleaseAsync(String keyConfig, object value)
        {
            try
            {
                if (string.IsNullOrEmpty(keyConfig))
                {
                    return false;
                }
                IDatabase db = _redisMultiplexer.GetDatabase(0);
                return await db.KeyDeleteAsync(keyConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LockReleaseAsync fail,key:{keyConfig},ex.Message:{ex.Message}, StackTrace:{ex.StackTrace}");
                return false;
            }
        }

        private static string ConvertToCacheValue(object value)
        {
            string cacheValue = "";
            if (value is string)
            {
                cacheValue = value.ToString();
            }
            else
            {
                cacheValue = JsonConvert.SerializeObject(value);
            }

            return cacheValue;
        }


    }
}
