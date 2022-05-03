using Microsoft.Extensions.Configuration;
using RedisPerfomTest.Services.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisPerfomTest.Services.Concreate
{
    public class RedisService : ICacheService
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        public RedisService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("RedisConfiguration:ConnectionString")?.Value;
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints =
                {
                    connectionString
                },
                AbortOnConnectFail = false, //Redis Offline
                AsyncTimeout = 10000, //Asyncronis request TimeOut 
                ConnectTimeout = 10000 //syncronis request TimeOut 
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(options);


        }

        public T Get<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            return _connectionMultiplexer.GetDatabase().StringGet(key);
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            string value = await _connectionMultiplexer.GetDatabase().StringGetAsync(key);
            return value.ToObject<T>();
        }

        public void Remove(string key)
        {
            _connectionMultiplexer.GetDatabase().KeyDelete(key);
        }

        public void Set(string key, string value)
        {
            _connectionMultiplexer.GetDatabase().StringSet(key, value);
        }

        public void Set<T>(string key, T value) where T : class
        {
            _connectionMultiplexer.GetDatabase().StringSet(key, value.ToJson());
            
        }

        public void Set(string key, object value, TimeSpan expiration)
        {
            _connectionMultiplexer.GetDatabase().StringSet(key, value.ToJson(), expiration);
        }

        public Task SetAsync(string key, object value)
        {
            return _connectionMultiplexer.GetDatabase().StringSetAsync(key, value.ToJson());
        }

        public Task SetAsync(string key, object value, TimeSpan expiration)
        {
            return _connectionMultiplexer.GetDatabase().StringSetAsync(key, value.ToJson(), expiration);
        }
    }
}
