using Keycloak.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Keycloak.Test
{
    public class KeycloakOptionsTest
    {
        [Fact]
        public void Bind_ReadsNestedConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("Keycloak:ServerUrl", "https://keycloak.example.com"),
                    new KeyValuePair<string, string>("Keycloak:Realm", "myrealm"),
                    new KeyValuePair<string, string>("Keycloak:ClientId", "admin-cli"),
                    new KeyValuePair<string, string>("Keycloak:ClientSecret", "secret"),
                })
                .Build();

            var options = KeycloakOptionsBinder.Bind(configuration);

            Assert.Equal("https://keycloak.example.com", options.ServerUrl);
            Assert.Equal("myrealm", options.Realm);
            Assert.Equal("admin-cli", options.ClientId);
            Assert.Equal("secret", options.ClientSecret);
            Assert.Equal("https://keycloak.example.com/realms/myrealm", options.Authority);
        }

        [Fact]
        public void Bind_FallsBackToLegacyFlatKeys()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("KeycloakServer", "https://legacy.example.com"),
                    new KeyValuePair<string, string>("Realm", "legacy-realm"),
                    new KeyValuePair<string, string>("ClientId", "legacy-client"),
                    new KeyValuePair<string, string>("ClientSecret", "legacy-secret"),
                })
                .Build();

            var options = KeycloakOptionsBinder.Bind(configuration);

            Assert.Equal("https://legacy.example.com", options.ServerUrl);
            Assert.Equal("legacy-realm", options.Realm);
        }

        [Fact]
        public void Validate_ThrowsWhenServerUrlMissing()
        {
            var options = new KeycloakOptions
            {
                Realm = "myrealm",
                ClientId = "client",
                ClientSecret = "secret",
            };

            Assert.Throws<KeycloakConfigurationException>(() => options.Validate());
        }
    }
}
