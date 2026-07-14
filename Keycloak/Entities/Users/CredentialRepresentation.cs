using Newtonsoft.Json;

namespace Keycloak.Entities.Users
{
    public class CredentialRepresentation
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "password";

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
    }
}
