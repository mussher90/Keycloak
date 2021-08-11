using Keycloak.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Keycloak.Clients
{
    public class BaseClient
    {
        protected string BaseURl;

        private HttpClient _client;

        public BaseClient()
        {
            _client = new HttpClient();
        }
        protected async Task<string> GetAsync(string EndPoint)
        {
            string result = null;

            HttpResponseMessage res = await _client.GetAsync(BaseURl + EndPoint);

            if (res.IsSuccessStatusCode)
            {
                result = await res.Content.ReadAsStringAsync();
            }

            return result;
        }

        protected async Task<string> PostAsync(string EndPoint, HttpContent content)
        {
            string result = null;

            HttpResponseMessage res = await _client.PostAsync(BaseURl + EndPoint, content);

            if (res.IsSuccessStatusCode)
            {
                result = await res.Content.ReadAsStringAsync();
            }

            return result;
        }

        protected ByteArrayContent FormatEntity(object KeycloakEntity)
        {
            var entityJson = JsonConvert.SerializeObject(KeycloakEntity);
            var buffer = Encoding.UTF8.GetBytes(entityJson);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }
    }
}
