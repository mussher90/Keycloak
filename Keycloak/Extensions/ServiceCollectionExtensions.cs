using Keycloak.Configuration;
using Keycloak.Services;
using Keycloak.Services.Keys;
using Keycloak.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeycloak(this IServiceCollection services, IConfiguration configuration)
        {
            var options = KeycloakOptionsBinder.Bind(configuration);
            options.Validate();

            services.AddSingleton(options);
            services.AddSingleton<IKeycloakClient, KeycloakClient>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IKeyService, KeyService>();
            services.AddSingleton<IRealmService, RealmService>();

            return services;
        }
    }
}
