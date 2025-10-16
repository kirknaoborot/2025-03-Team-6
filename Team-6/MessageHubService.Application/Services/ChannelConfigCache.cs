using Microsoft.Extensions.Caching.Memory;

namespace MessageHubService.Application.Services
{
    public class ChannelConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string ChannelType { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public interface IChannelConfigCache
    {
        void Upsert(ChannelConfig cfg);
        bool TryGet(int id, out ChannelConfig cfg);
        IReadOnlyCollection<ChannelConfig> GetAll();
    }

    public class ChannelConfigCache : IChannelConfigCache
    {
        private readonly IMemoryCache _cache;
        private const string Prefix = "channel-config-";
        private const string AllKey = "channel-config-keys";

        public ChannelConfigCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Upsert(ChannelConfig cfg)
        {
            var key = Key(cfg.Id);
            _cache.Set(key, cfg, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });

            var keys = _cache.GetOrCreate(AllKey, _ => new HashSet<int>());
            keys.Add(cfg.Id);
            _cache.Set(AllKey, keys);
        }

        public bool TryGet(int id, out ChannelConfig cfg)
        {
            var ok = _cache.TryGetValue(Key(id), out ChannelConfig? value);
            cfg = value;
            return ok && value != null;
        }

        public IReadOnlyCollection<ChannelConfig> GetAll()
        {
            var keys = _cache.Get<HashSet<int>>(AllKey) ?? new HashSet<int>();
            var list = new List<ChannelConfig>();
            foreach (var id in keys)
            {
                if (_cache.TryGetValue(Key(id), out ChannelConfig? cfg) && cfg != null)
                    list.Add(cfg);
            }
            return list;
        }

        private static string Key(int id) => $"{Prefix}{id}";
    }
}

