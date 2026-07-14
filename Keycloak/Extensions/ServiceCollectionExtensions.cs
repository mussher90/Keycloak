using Keycloak.Configuration;
using Keycloak.Services;
using Keycloak.Services.Keys;
using Keycloak.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Keycloak.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string HttpClientName = "Keycloak";

        public static IServiceCollection AddKeycloak(this IServiceCollection services, IConfiguration configuration)
        {
            var options = KeycloakOptionsBinder.Bind(configuration);
            options.Validate();

            services.AddSingleton(options);
            services.AddSingleton<IOptions<KeycloakOptions>>(_ => Options.Create(options));

            services.AddHttpClient(HttpClientName, (serviceProvider, client) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                client.BaseAddress = new Uri(keycloakOptions.ServerUrl.TrimEnd('/') + "/");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddSingleton<IKeycloakClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(HttpClientName);
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                return new KeycloakClient(httpClient, keycloakOptions);
            });

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IKeyService, KeyService>();
            services.AddSingleton<IRealmService, RealmService>();

            return services;
        }
    }
}
