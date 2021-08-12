using Keycloak.Entities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public class KeyService : BaseService
    {
        private readonly string EndPoint = "/protocol/openid-connect/certs";

        public KeyService(HttpClient client, Uri baseUri) : base(client, baseUri)
        {
            _baseUri = baseUri;
            _client = client;
        }

        public async Task<RealmKeys> GetKeys()
        {
            var response = await _client.GetAsync(_baseUri + EndPoint);

            if (response.IsSuccessStatusCode)
            {
                var keys = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RealmKeys>(keys);
            }

            return null;
        }
    }
}
