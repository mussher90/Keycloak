using Newtonsoft.Json;

namespace Keycloak.Entities
{
    public class OidcToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long Lifespan { get; set; }

        [JsonProperty("refresh_expires_in")]
        public long RefreshLifespan { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("not-before-policy")]
        public long NbPolicy { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
