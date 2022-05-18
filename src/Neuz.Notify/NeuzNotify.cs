using System;
using Microsoft.Extensions.Caching.Memory;
using Neuz.Notify.WXWork;

namespace Neuz.Notify
{
    public class NeuzNotify
    {
        public static WXWorkSender WXWork => new WXWorkSender();
    }

    public class NotifyCache
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public static bool Set(string key, object value, DateTimeOffset offset)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Cache.Set(key, value, offset);
            return Cache.TryGetValue(key, out _);
        }

        public static string? Get(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return Cache.Get(key)?.ToString();
        }
    }
}