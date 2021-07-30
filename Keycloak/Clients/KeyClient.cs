using Keycloak.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak
{
    public class KeyClient
    {
        private const string EndPoint = "/protocol/openid-connect/certs";

        private string BaseURl;

        public KeyClient(string baseUrl)
        {
            BaseURl = baseUrl;
        }

        private async Task<string> GetAsync()
        {
            HttpClient client = new HttpClient();

            string result = null;

            HttpResponseMessage res = await client.GetAsync(BaseURl + EndPoint);

            if (res.IsSuccessStatusCode)
            {
                result = await res.Content.ReadAsStringAsync();
            }

            return result;
        }

        public RealmKeys GetKeys()
        {
            return JsonConvert.DeserializeObject<RealmKeys>(GetAsync().Result);
        }
    }
}
