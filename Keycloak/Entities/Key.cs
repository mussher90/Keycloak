using Newtonsoft.Json;
using System.Collections.Generic;

namespace Keycloak.Entities
{
    public class Key
    {
        [JsonProperty("kid")]
        public string KeyId { get; set; }

        [JsonProperty("kty")]
        public string KeyType { get; set; }

        [JsonProperty("alg")]
        public string Algorithm { get; set; }

        [JsonProperty("use")]
        public string Use { get; set; }

        [JsonProperty("n")]
        public string Modulus { get; set; }

        [JsonProperty("e")]
        public string Exponent { get; set; }

        [JsonProperty("x5c")] 
        public List<string> Certificate { get; set; }

        [JsonProperty("x5t")]
        public string Thumbprint { get; set; }

        [JsonProperty("x5t#S256")]
        public string OtherThing { get; set; }
    }

    public class RealmKeys
    {
        public List<Key> Keys { get; set; }
    }
}
