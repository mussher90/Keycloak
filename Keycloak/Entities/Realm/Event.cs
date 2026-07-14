using Newtonsoft.Json;
using System.Collections.Generic;

namespace Keycloak.Entities.Realm
{
    public class Event
    {
        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("realmId")]
        public string RealmId { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("details")]
        public Dictionary<string, string> Details { get; set; }
    }
}
