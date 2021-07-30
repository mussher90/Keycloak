using Newtonsoft.Json;
using System.Collections.Generic;

namespace Keycloak.Entities
{
    public class AccessToken
    {
        [JsonProperty("exp")]
        public long Expiry { get; set; }

        [JsonProperty("iat")]
        public long IssuedAt { get; set; }

        [JsonProperty("auth_time")]
        public long AuthorizationTime { get; set; }

        [JsonProperty("jti")]
        public string Jti { get; set; }

        [JsonProperty("iss")]
        public string Issuer { get; set; }

        [JsonProperty("aud")]
        public string Audience { get; set; }

        [JsonProperty("sub")]
        public string KeycloakUserId { get; set; }

        [JsonProperty("typ")]
        public string TokenType { get; set; }

        [JsonProperty("azp")]
        public string AuthorizingParty { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("session_state")]
        public string SessionId { get; set; }

        [JsonProperty("acr")]
        public string Acr { get; set; }

        [JsonProperty("allowed-origins")]
        public string AllowedOrigins { get; set; }

        [JsonProperty("realm_access")]
        public RealmRoles RealmAccess { get; set; }

        [JsonProperty("resource_access")]
        public Dictionary<string, RealmRoles> ResourceAccess { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }

    public class RealmRoles
    {
        public List<string> Roles { get; set; }
    }
}
