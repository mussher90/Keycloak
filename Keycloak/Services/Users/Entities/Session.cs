using Newtonsoft.Json;

namespace Keycloak.Services.Users.Entities
{
    public class Session
    {
        [JsonProperty("id")]
        public string SessionId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("userId")]
        public string KeycloakUserId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("start")]
        public long StartTime { get; set; }

        [JsonProperty("lastAccess")]
        public long LastAccess { get; set; }

        [JsonProperty("clients")]
        public object Clients { get; set; }
    }
}
