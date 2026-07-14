using Keycloak.Entities.Keys;
using Newtonsoft.Json;
using Xunit;

namespace Keycloak.Test
{
    public class RealmKeysTest
    {
        [Fact]
        public void DeserializesKeysFromJwksResponse()
        {
            var json = @"{
                ""keys"": [
                    {
                        ""kid"": ""abc123"",
                        ""kty"": ""RSA"",
                        ""alg"": ""RS256"",
                        ""use"": ""sig"",
                        ""n"": ""modulus"",
                        ""e"": ""AQAB""
                    }
                ]
            }";

            var realmKeys = JsonConvert.DeserializeObject<RealmKeys>(json);

            Assert.NotNull(realmKeys.Keys);
            Assert.Single(realmKeys.Keys);
            Assert.Equal("abc123", realmKeys.Keys[0].KeyId);
            Assert.Equal("RSA", realmKeys.Keys[0].KeyType);
        }
    }
}
