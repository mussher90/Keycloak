using Keycloak.Entities.Realm;
using Newtonsoft.Json;
using Xunit;

namespace Keycloak.Test
{
    public class EventTest
    {
        [Fact]
        public void DeserializesFromKeycloakFormat()
        {
            var json = @"[{
                ""time"": 1630876970000,
                ""type"": ""LOGIN"",
                ""realmId"": ""myrealm"",
                ""clientId"": ""my-client"",
                ""userId"": ""user-123"",
                ""sessionId"": ""session-456"",
                ""ipAddress"": ""127.0.0.1"",
                ""error"": null,
                ""details"": { ""auth_method"": ""openid-connect"" }
            }]";

            var events = JsonConvert.DeserializeObject<System.Collections.Generic.List<Event>>(json);

            Assert.Single(events);
            Assert.Equal("LOGIN", events[0].Type);
            Assert.Equal("my-client", events[0].ClientId);
            Assert.Equal("user-123", events[0].UserId);
            Assert.Equal("127.0.0.1", events[0].IpAddress);
            Assert.Equal("openid-connect", events[0].Details["auth_method"]);
        }
    }
}
