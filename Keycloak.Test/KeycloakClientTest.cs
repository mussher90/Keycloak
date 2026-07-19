using Keycloak.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Keycloak.Test
{
    public class KeycloakClientTest
    {
        [Fact]
        public async Task Send_ConcurrentRequestsWithoutCachedToken_FetchesTokenOnce()
        {
            var tokenRequestCount = 0;
            var client = CreateClient((request, _) =>
            {
                if (IsTokenRequest(request))
                {
                    Interlocked.Increment(ref tokenRequestCount);
                    Thread.Sleep(50);

                    return TokenResponse(DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds());
                }

                return JsonResponse(HttpStatusCode.OK, "{}");
            });

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(SendProtectedRequest(client));
            }

            await Task.WhenAll(tasks);

            Assert.Equal(1, tokenRequestCount);
        }

        [Fact]
        public async Task Send_ConcurrentRequestsWithExpiredToken_RefreshesTokenOnce()
        {
            var tokenRequestCount = 0;
            var client = CreateClient((request, _) =>
            {
                if (IsTokenRequest(request))
                {
                    var count = Interlocked.Increment(ref tokenRequestCount);
                    Thread.Sleep(50);

                    var expiry = count == 1
                        ? DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds()
                        : DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();

                    return TokenResponse(expiry);
                }

                return JsonResponse(HttpStatusCode.OK, "{}");
            });

            await SendProtectedRequest(client);
            Assert.Equal(1, tokenRequestCount);

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(SendProtectedRequest(client));
            }

            await Task.WhenAll(tasks);

            Assert.Equal(2, tokenRequestCount);
        }

        [Fact]
        public async Task Send_WithValidCachedToken_DoesNotRefetchToken()
        {
            var tokenRequestCount = 0;
            var client = CreateClient((request, _) =>
            {
                if (IsTokenRequest(request))
                {
                    Interlocked.Increment(ref tokenRequestCount);
                    return TokenResponse(DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds());
                }

                return JsonResponse(HttpStatusCode.OK, "{}");
            });

            await SendProtectedRequest(client);
            Assert.Equal(1, tokenRequestCount);

            await SendProtectedRequest(client);

            Assert.Equal(1, tokenRequestCount);
        }

        private static KeycloakClient CreateClient(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(handler))
            {
                BaseAddress = new Uri("https://keycloak.test/"),
            };

            return new KeycloakClient(httpClient, new KeycloakOptions
            {
                ServerUrl = "https://keycloak.test",
                Realm = "test-realm",
                ClientId = "test-client",
                ClientSecret = "test-secret",
            });
        }

        private static Task<HttpResponseMessage> SendProtectedRequest(KeycloakClient client)
        {
            return client.Send(new HttpRequestMessage(HttpMethod.Get, "admin/realms/test-realm/users"));
        }

        private static bool IsTokenRequest(HttpRequestMessage request)
        {
            return request.RequestUri.ToString().Contains("protocol/openid-connect/token");
        }

        private static HttpResponseMessage TokenResponse(long expiryUnixSeconds)
        {
            var accessToken = CreateTestJwt(expiryUnixSeconds);
            var expiresIn = Math.Max(0, (int)(expiryUnixSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            var body = $"{{\"access_token\":\"{accessToken}\",\"expires_in\":{expiresIn},\"token_type\":\"Bearer\"}}";

            return JsonResponse(HttpStatusCode.OK, body);
        }

        private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string body)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
        }

        private static string CreateTestJwt(long expiryUnixSeconds)
        {
            var header = Base64UrlEncode("{}");
            var payload = Base64UrlEncode($"{{\"exp\":{expiryUnixSeconds}}}");

            return $"{header}.{payload}.signature";
        }

        private static string Base64UrlEncode(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_handler(request, cancellationToken));
            }
        }
    }
}
