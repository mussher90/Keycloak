using Keycloak.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Keycloak.Middleware
{
    internal static class RealmKeysCache
    {
        public const string KeyPrefix = "Keycloak:RealmKeys:";

        public static string GetCacheKey(string realm)
        {
            return KeyPrefix + realm;
        }

        public static MemoryCacheEntryOptions CreateEntryOptions(KeycloakOptions options)
        {
            var cacheSeconds = options.RealmKeysCacheSeconds;

            if (cacheSeconds <= 0)
            {
                cacheSeconds = 3600;
            }

            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheSeconds),
            };
        }
    }
}
