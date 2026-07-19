#if NET8_0_OR_GREATER
using Keycloak.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Keycloak.Extensions
{
    public static class KeycloakAuthenticationExtensions
    {
        public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureKeycloakJwtBearerOptions>();

            return services;
        }

        public static IApplicationBuilder UseKeycloakAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }

    internal sealed class ConfigureKeycloakJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly KeycloakOptions _options;

        public ConfigureKeycloakJwtBearerOptions(KeycloakOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(JwtBearerOptions options)
        {
            options.Authority = _options.Authority;
            options.RequireHttpsMetadata = _options.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            options.TokenValidationParameters.ValidateAudience = false;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer = _options.Authority;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(_options.ServerSkew);

            if (!_options.ValidateClientId)
            {
                return;
            }

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var authorizedParty = context.Principal?.FindFirst("azp")?.Value;

                    if (authorizedParty != _options.ClientId)
                    {
                        context.Fail("The token authorized party (azp) does not match the configured client ID.");
                    }

                    return Task.CompletedTask;
                },
            };
        }
    }
}
#endif
