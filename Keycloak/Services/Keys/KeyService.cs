using Keycloak.Entities.Keys;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services.Keys
{
    public static class KeyService
    {
        public static async Task<RealmKeys> GetKeysAsync(IKeycloakClient client)
        {
            var keyEndpoint = $"auth/realms/{client.Realm}/protocol/openid-connect/certs";

            var message = ApiHelper.ConstructRequest(HttpMethod.Get, keyEndpoint);

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
