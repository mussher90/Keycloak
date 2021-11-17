using Newtonsoft.Json;
using System.Collections.Generic;

namespace Keycloak.Entities
{
    public class Payload
    {
        [JsonProperty("exp")]
        public long Expiry { get; set; }

        [JsonProperty("iat")]
        public long IssuedAt { get; set; }

        [JsonProperty("jti")]
        public string Jti { get; set; }

        [JsonProperty("iss")]
        public string Issuer { get; set; }

        [JsonProperty("aud")]
        public object Audience { get; set; }

        [JsonProperty("sub")]
        public string KeycloakUserId { get; set; }

        [JsonProperty("typ")]
        public string TokenType { get; set; }

        [JsonProperty("azp")]
        public string AuthorizingParty { get; set; }

        [JsonProperty("session_state")]
        public string SessionState { get; set; }

        [JsonProperty("acr")]
        public string Acr { get; set; }

        [JsonProperty("sid")]
        public string SessionId { get; set; }

        [JsonProperty("allowed-origins")]
        public List<string> AllowedOrigins { get; set; }
    }
}
