using Keycloak.Configuration;
using Keycloak.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace Keycloak.Test
{
    public class KeycloakAuthenticationExtensionsTest
    {
        [Fact]
        public void AddKeycloakAuthentication_ConfiguresJwtBearerFromKeycloakOptions()
        {
            var services = new ServiceCollection();
            var options = new KeycloakOptions
            {
                ServerUrl = "https://keycloak.example.com",
                Realm = "myrealm",
                ClientId = "my-service",
                ClientSecret = "secret",
                ServerSkew = 30,
                ValidateClientId = true,
            };

            services.AddSingleton(options);
            services.AddKeycloakAuthentication();

            using var provider = services.BuildServiceProvider();
            var jwtOptions = provider
                .GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme);

            Assert.Equal(options.Authority, jwtOptions.Authority);
            Assert.Equal(options.Authority, jwtOptions.TokenValidationParameters.ValidIssuer);
            Assert.Equal(TimeSpan.FromSeconds(30), jwtOptions.TokenValidationParameters.ClockSkew);
            Assert.False(jwtOptions.TokenValidationParameters.ValidateAudience);
            Assert.NotNull(jwtOptions.Events);
        }

        [Fact]
        public void AddKeycloakAuthentication_SkipsAzpValidation_WhenDisabled()
        {
            var services = new ServiceCollection();
            var options = new KeycloakOptions
            {
                ServerUrl = "https://keycloak.example.com",
                Realm = "myrealm",
                ClientId = "my-service",
                ClientSecret = "secret",
                ValidateClientId = false,
            };

            services.AddSingleton(options);
            services.AddKeycloakAuthentication();

            using var provider = services.BuildServiceProvider();
            var jwtOptions = provider
                .GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>()
                .Get(JwtBearerDefaults.AuthenticationScheme);

            Assert.Null(jwtOptions.Events);
        }
    }
}
