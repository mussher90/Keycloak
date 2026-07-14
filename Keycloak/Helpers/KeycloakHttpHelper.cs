using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Helpers
{
    internal static class KeycloakHttpHelper
    {
        public static async Task<T> SendAndDeserializeAsync<T>(
            IKeycloakClient client,
            HttpRequestMessage request,
            bool requiresAccessToken = true)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken).ConfigureAwait(false))
            {
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new KeycloakApiException(response.StatusCode, body, request.RequestUri);
                }

                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        public static async Task SendAsync(
            IKeycloakClient client,
            HttpRequestMessage request,
            bool requiresAccessToken = true)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new KeycloakApiException(response.StatusCode, body, request.RequestUri);
                }
            }
        }

        public static async Task<string> SendAndGetCreatedResourceIdAsync(
            IKeycloakClient client,
            HttpRequestMessage request,
            bool requiresAccessToken = true)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new KeycloakApiException(response.StatusCode, body, request.RequestUri);
                }

                if (response.Headers.Location == null)
                {
                    throw new KeycloakApiException(
                        response.StatusCode,
                        "Successful response did not include a Location header.",
                        request.RequestUri);
                }

                return response.Headers.Location.Segments.Last();
            }
        }
    }
}
