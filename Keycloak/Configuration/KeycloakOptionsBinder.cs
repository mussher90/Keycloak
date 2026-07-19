using Microsoft.Extensions.Configuration;
using System;

namespace Keycloak.Configuration
{
    public static class KeycloakOptionsBinder
    {
        public static KeycloakOptions Bind(IConfiguration configuration)
        {
            var section = configuration.GetSection(KeycloakOptions.SectionName);

            var options = new KeycloakOptions
            {
                ServerUrl = FirstNonEmpty(section["ServerUrl"], configuration["KeycloakServer"]),
                Realm = FirstNonEmpty(section["Realm"], configuration["Realm"]),
                ClientId = FirstNonEmpty(section["ClientId"], configuration["ClientId"]),
                ClientSecret = FirstNonEmpty(section["ClientSecret"], configuration["ClientSecret"]),
            };

            if (TryParseInt(FirstNonEmpty(section["ServerSkew"], configuration["ServerSkew"]), out var serverSkew))
            {
                options.ServerSkew = serverSkew;
            }

            if (TryParseBool(FirstNonEmpty(section["ValidateClientId"], configuration["ValidateClientId"]), out var validateClientId))
            {
                options.ValidateClientId = validateClientId;
            }

            return options;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private static bool TryParseInt(string value, out int result)
        {
            result = 0;

            return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result);
        }

        private static bool TryParseBool(string value, out bool result)
        {
            result = false;

            return !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out result);
        }
    }
}
