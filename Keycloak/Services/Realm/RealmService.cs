using Keycloak.Services.Realm.Entites;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public static class RealmService
    {
        public static async Task<List<Event>> GetRealmEventsAsync(IKeycloakClient client, EventQuery eventQuery)
        {
            var queryParameters = eventQuery.GetQueryString();
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/events{queryParameters}";

            var message = ApiHelper.ConstructRequest(HttpMethod.Get, apiEndpoint);

            var response = await client.Send(message, true);

            if (response.IsSuccessStatusCode)
            {
                var eventData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                using var reader = new StreamReader(eventData);
                return JsonConvert.DeserializeObject<List<Event>>(reader.ReadToEnd());
            }

            return null;
        }
    }
}
