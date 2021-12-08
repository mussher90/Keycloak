using Newtonsoft.Json;

namespace Keycloak.Entities
{
    public class Header
    {
        [JsonProperty("alg")]
        public string Algorithm { get; set; }

        [JsonProperty("typ")]
        public string Type { get; set; }

        [JsonProperty("kid")]
        public string KeyId { get; set; }
    }
}
