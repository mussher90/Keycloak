namespace Keycloak.Helpers
{
    public static class KeycloakUriHelper
    {
        /// <summary>
        /// Builds the JWT issuer URL for a realm (matches the "iss" claim in Keycloak tokens).
        /// Example: https://keycloak.example.com/realms/myrealm
        /// </summary>
        public static string GetIssuer(string serverUrl, string realm)
        {
            return $"{serverUrl.TrimEnd('/')}/realms/{realm}";
        }

        public static string GetTokenEndpoint(string realm)
        {
            return $"realms/{realm}/protocol/openid-connect/token";
        }

        public static string GetCertsEndpoint(string realm)
        {
            return $"realms/{realm}/protocol/openid-connect/certs";
        }

        public static string GetAdminRealmPath(string realm)
        {
            return $"admin/realms/{realm}";
        }
    }
}
