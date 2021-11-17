using Keycloak.Entities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public static class KeyService
    {
        public static async Task<RealmKeys> GetKeys(IKeycloakClient client)
        {
            var keyEndpoint = $"auth/realms/{client.Realm}/protocol/openid-connect/certs";

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(keyEndpoint, UriKind.Relative)
            };

            var response = await client.Send(message, false).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var keys = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<RealmKeys>(keys);
            }

            return null;
        }
    }
}
