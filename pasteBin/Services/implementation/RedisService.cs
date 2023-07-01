using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using pasteBin.Areas.Home.Models;
using pasteBin.Services.Interfaces;

namespace pasteBin.Services.implementation
{
    public class RedisService : IRedisCache
    {
        private readonly IDistributedCache cache;
        private readonly int timeInterval = 1;

        public PasteModel? Get(string key)
        {
            PasteModel? paste = null;
            string? pasteString = cache.GetString(key);

            if (pasteString != null)
                paste = JsonSerializer.Deserialize<PasteModel>(pasteString);

            return paste;
        }

        public List<PasteModel> GetAll()
        {
            List<string> keysList = new();
            List<PasteModel> pasts = new();

            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("127.0.0.1:6379"))
            {
                IDatabase redisDb = redis.GetDatabase();

                var keys = redis.GetServer("127.0.0.1", 6379).Keys(pattern: "*");
                keysList.AddRange(keys.Select(key => (string)key).ToList());
            }

            foreach (string key in keysList)
            {
                PasteModel paste = Get(key.Replace("local", string.Empty));

                pasts.Add(paste);
            }

            return pasts;
        }

        public void Set(int timeInterval, IEnumerable<PasteModel> pasts)
        {
            foreach (PasteModel paste in pasts)
            {
                if (GetAll().Any())
                {
                    Update(paste);
                }
                else
                {
                    string pasteString = JsonSerializer.Serialize(paste);

                    cache.SetString(paste.Hash, pasteString, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(timeInterval)
                    });
                }
            }
        }

        public void Update(PasteModel paste)
        {
            cache.Remove(paste.Hash);

            string pasteString = JsonSerializer.Serialize(paste);

            cache.SetString(paste.Hash, pasteString, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(timeInterval)
            });
        }

        public void Remove(IEnumerable<PasteModel> pasts)
        {
            foreach (PasteModel paste in pasts)
            {
                PasteModel? cachePaste = Get(paste.Hash);

                if (cachePaste != null)
                    cache.Remove(paste.Hash);
            }
        }

        public RedisService(IServiceScopeFactory factory)
        {
            cache = factory.CreateScope().ServiceProvider.GetRequiredService<IDistributedCache>();
        }
    }
}
