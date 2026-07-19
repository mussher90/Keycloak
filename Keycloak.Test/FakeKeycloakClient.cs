using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak.Test
{
    public class FakeKeycloakClient : IKeycloakClient
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeKeycloakClient(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        public string Realm { get; set; } = "test-realm";

        public string ServerUrl { get; set; } = "https://keycloak.test";

        public HttpRequestMessage LastRequest { get; private set; }

        public bool LastRequiresAccessToken { get; private set; }

        public CancellationToken LastCancellationToken { get; private set; }

        public Task<HttpResponseMessage> Send(
            HttpRequestMessage message,
            bool requiresAccessToken = true,
            CancellationToken cancellationToken = default)
        {
            LastRequest = message;
            LastRequiresAccessToken = requiresAccessToken;
            LastCancellationToken = cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_handler(message));
        }

        public static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string body)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body),
            };
        }

        public static HttpResponseMessage CreatedResponse(string location)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(location, UriKind.RelativeOrAbsolute);
            return response;
        }
    }
}
