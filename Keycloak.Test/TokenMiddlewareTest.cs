using Keycloak.Configuration;
using Keycloak.Entities;
using Keycloak.Entities.Keys;
using Keycloak.Middleware;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Xunit;

namespace Keycloak.Test
{
    public class TokenMiddlewareTest
    {
        [Fact]
        public void RealmKeysCache_GetCacheKey_IncludesRealmName()
        {
            Assert.Equal("Keycloak:RealmKeys:myrealm", RealmKeysCache.GetCacheKey("myrealm"));
        }

        [Fact]
        public void RealmKeysCache_CreateEntryOptions_UsesConfiguredDuration()
        {
            var options = new KeycloakOptions { RealmKeysCacheSeconds = 120 };

            var entryOptions = RealmKeysCache.CreateEntryOptions(options);

            Assert.Equal(System.TimeSpan.FromSeconds(120), entryOptions.AbsoluteExpirationRelativeToNow);
        }

        [Fact]
        public void RealmKeysCache_CreateEntryOptions_FallsBackToDefault_WhenZeroOrNegative()
        {
            var options = new KeycloakOptions { RealmKeysCacheSeconds = 0 };

            var entryOptions = RealmKeysCache.CreateEntryOptions(options);

            Assert.Equal(System.TimeSpan.FromSeconds(3600), entryOptions.AbsoluteExpirationRelativeToNow);
        }

        [Fact]
        public void BuildValidationParameters_SetsIssuerClientAndSkew()
        {
            var options = new KeycloakOptions
            {
                ServerUrl = "https://keycloak.example.com",
                Realm = "myrealm",
                ClientId = "my-client",
                ClientSecret = "secret",
                ServerSkew = 30,
                ValidateClientId = true,
            };

            var middleware = new TokenMiddleware(null, null, null, options);
            var realmKeys = new RealmKeys { Keys = new List<Key>() };

            var parameters = middleware.BuildValidationParameters("token", realmKeys);

            Assert.Equal("https://keycloak.example.com/realms/myrealm", parameters.Issuer);
            Assert.Equal("my-client", parameters.Client);
            Assert.True(parameters.CheckClient);
            Assert.Equal(30, parameters.ServerSkew);
        }

        [Fact]
        public void BuildValidationParameters_SkipsClientValidation_WhenDisabled()
        {
            var options = new KeycloakOptions
            {
                ServerUrl = "https://keycloak.example.com",
                Realm = "myrealm",
                ClientId = "my-client",
                ClientSecret = "secret",
                ValidateClientId = false,
            };

            var middleware = new TokenMiddleware(null, null, null, options);
            var parameters = middleware.BuildValidationParameters("token", new RealmKeys());

            Assert.False(parameters.CheckClient);
        }
    }
}
