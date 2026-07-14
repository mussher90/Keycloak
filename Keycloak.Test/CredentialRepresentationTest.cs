using Keycloak.Entities.Users;
using Newtonsoft.Json;
using Xunit;

namespace Keycloak.Test
{
    public class CredentialRepresentationTest
    {
        [Fact]
        public void SerializesTypeAsPassword()
        {
            var credential = new CredentialRepresentation
            {
                Value = "secret123",
                Temporary = true
            };

            var json = JsonConvert.SerializeObject(credential);

            Assert.Contains("\"type\":\"password\"", json);
            Assert.Contains("\"value\":\"secret123\"", json);
            Assert.Contains("\"temporary\":true", json);
        }

        [Fact]
        public void DeserializesFromKeycloakFormat()
        {
            var json = "{\"type\":\"password\",\"value\":\"secret123\",\"temporary\":false}";

            var credential = JsonConvert.DeserializeObject<CredentialRepresentation>(json);

            Assert.Equal("password", credential.Type);
            Assert.Equal("secret123", credential.Value);
            Assert.False(credential.Temporary);
        }
    }
}
