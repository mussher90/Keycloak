using Keycloak.Services.Keys;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Keycloak.Test
{
    public class KeyServiceTest
    {
        [Fact]
        public async Task GetKeysAsync_UsesCertsEndpointWithoutAccessToken()
        {
            var client = new FakeKeycloakClient(_ =>
                FakeKeycloakClient.JsonResponse(
                    HttpStatusCode.OK,
                    "{\"keys\":[{\"kid\":\"abc\",\"kty\":\"RSA\",\"alg\":\"RS256\",\"use\":\"sig\",\"n\":\"modulus\",\"e\":\"AQAB\"}]}"));
            var service = new KeyService(client);

            var keys = await service.GetKeysAsync();

            Assert.False(client.LastRequiresAccessToken);
            Assert.Equal("realms/test-realm/protocol/openid-connect/certs", client.LastRequest.RequestUri.ToString());
            Assert.Single(keys.Keys);
            Assert.Equal("abc", keys.Keys[0].KeyId);
        }
    }
}
