using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak.Helpers
{
    internal static class KeycloakHttpHelper
    {
        public static async Task<T> SendAndDeserializeAsync<T>(
            IKeycloakClient client,
            HttpRequestMessage request,
            bool requiresAccessToken = true,
            CancellationToken cancellationToken = default)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken, cancellationToken).ConfigureAwait(false))
            {
                var body = await ReadContentAsStringAsync(response.Content, cancellationToken).ConfigureAwait(false);

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
            bool requiresAccessToken = true,
            CancellationToken cancellationToken = default)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await ReadContentAsStringAsync(response.Content, cancellationToken).ConfigureAwait(false);
                    throw new KeycloakApiException(response.StatusCode, body, request.RequestUri);
                }
            }
        }

        public static async Task<string> SendAndGetCreatedResourceIdAsync(
            IKeycloakClient client,
            HttpRequestMessage request,
            bool requiresAccessToken = true,
            CancellationToken cancellationToken = default)
        {
            using (request)
            using (var response = await client.Send(request, requiresAccessToken, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await ReadContentAsStringAsync(response.Content, cancellationToken).ConfigureAwait(false);
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

        private static async Task<string> ReadContentAsStringAsync(
            HttpContent content,
            CancellationToken cancellationToken)
        {
#if NET8_0_OR_GREATER
            return await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            cancellationToken.ThrowIfCancellationRequested();
            return await content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        }
    }
}
