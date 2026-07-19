using Keycloak.Helpers;

namespace Keycloak.Configuration
{
    public class KeycloakOptions
    {
        public const string SectionName = "Keycloak";

        public string ServerUrl { get; set; }

        public string Realm { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        /// <summary>
        /// Clock skew in seconds applied when validating token expiry. Defaults to 0.
        /// </summary>
        public int ServerSkew { get; set; }

        /// <summary>
        /// When true, JWT bearer authentication validates that the token's azp claim matches ClientId.
        /// </summary>
        public bool ValidateClientId { get; set; } = true;

        public string Authority =>
            string.IsNullOrEmpty(ServerUrl) || string.IsNullOrEmpty(Realm)
                ? null
                : KeycloakUriHelper.GetIssuer(ServerUrl, Realm);

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ServerUrl))
            {
                throw new KeycloakConfigurationException("Keycloak ServerUrl is required.");
            }

            if (string.IsNullOrWhiteSpace(Realm))
            {
                throw new KeycloakConfigurationException("Keycloak Realm is required.");
            }

            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new KeycloakConfigurationException("Keycloak ClientId is required.");
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new KeycloakConfigurationException("Keycloak ClientSecret is required.");
            }
        }
    }
}
