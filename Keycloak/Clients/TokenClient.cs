using Keycloak.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Keycloak.Clients
{
    public class TokenClient : BaseClient
    {
        private readonly string EndPoint = "/protocol/openid-connect/token";

        public TokenClient(string baseUrl)
        {
            BaseURl = baseUrl;
        }

        public OidcToken GetToken()
        {
            var entityJson = "{\"grant_type\" : \"client_credentials\", \"clientid\":\"blogBE\",\"client_secret\":\"c26ac412-4a55-4384-89aa-f29dd088dfd6\"}";
            var buffer = Encoding.UTF8.GetBytes(entityJson);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var result = PostAsync(EndPoint, byteContent).Result;

            return JsonConvert.DeserializeObject<OidcToken>(result);
        }
    }
}
