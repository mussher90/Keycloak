using Newtonsoft.Json;

namespace Keycloak.Services.Users.Entities
{
    public class CredentialRepresentation
    {
        [JsonProperty("type")]
        public const string Type = "Password";

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("temporary")]
        public string Temporary { get; set; }
    }
}
