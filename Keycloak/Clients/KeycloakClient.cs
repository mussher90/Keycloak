using Keycloak.Configuration;
using Keycloak.Entities;
using Keycloak.Helpers;
using Keycloak.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientSecret;
        private readonly string _clientId;
        private readonly int _serverSkew;
        private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);
        private OidcToken _token;
        private DateTimeOffset _tokenExpiresAt;

        public KeycloakClient(HttpClient httpClient, IOptions<KeycloakOptions> options)
            : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)))
        {
        }

        public KeycloakClient(HttpClient httpClient, KeycloakOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Validate();

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(NormalizeServerUrl(options.ServerUrl));
            }

            ServerUrl = options.ServerUrl;
            Realm = options.Realm;
            _clientId = options.ClientId;
            _clientSecret = options.ClientSecret;
            _serverSkew = options.ServerSkew;
        }

        public KeycloakClient(KeycloakOptions options)
            : this(CreateDefaultHttpClient(options), options)
        {
        }

        public KeycloakClient(IConfiguration configuration)
            : this(KeycloakOptionsBinder.Bind(configuration))
        {
        }

        public KeycloakClient(Uri host, string realm, string clientId, string clientSecret)
            : this(new KeycloakOptions
            {
                ServerUrl = host.ToString(),
                Realm = realm,
                ClientId = clientId,
                ClientSecret = clientSecret,
            })
        {
        }

        public string Realm { get; }

        public string ServerUrl { get; }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage message, bool requiresAccessToken = true)
        {
            if (requiresAccessToken)
            {
                await SetToken().ConfigureAwait(false);

                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);
            }

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        private bool ValidToken()
        {
            return _token != null &&
                   !string.IsNullOrEmpty(_token.AccessToken) &&
                   DateTimeOffset.UtcNow < _tokenExpiresAt;
        }

        private void SetCachedToken(OidcToken token)
        {
            _token = token;
            _tokenExpiresAt = CalculateTokenExpiry(token);
        }

        private DateTimeOffset CalculateTokenExpiry(OidcToken token)
        {
            DateTimeOffset? lifespanExpiry = null;
            DateTimeOffset? jwtExpiry = null;

            if (token.Lifespan > 0)
            {
                lifespanExpiry = DateTimeOffset.UtcNow.AddSeconds(token.Lifespan);
            }

            if (!string.IsNullOrEmpty(token.AccessToken) &&
                TokenValidator.TryGetAccessTokenExpiry(token.AccessToken, out var parsedJwtExpiry))
            {
                jwtExpiry = parsedJwtExpiry;
            }

            if (!lifespanExpiry.HasValue && !jwtExpiry.HasValue)
            {
                return DateTimeOffset.MinValue;
            }

            var expiry = lifespanExpiry ?? jwtExpiry.Value;

            if (jwtExpiry.HasValue && jwtExpiry.Value < expiry)
            {
                expiry = jwtExpiry.Value;
            }

            return expiry.AddSeconds(_serverSkew);
        }

        private async Task SetToken()
        {
            if (ValidToken())
            {
                return;
            }

            await _tokenLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (ValidToken())
                {
                    return;
                }

                var apiEndpoint = KeycloakUriHelper.GetTokenEndpoint(Realm);

                var grant = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                };

                var content = new FormUrlEncodedContent(grant);

                var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = content,
                    RequestUri = new Uri(apiEndpoint, UriKind.Relative),
                };

                var response = await Send(message, false).ConfigureAwait(false);

                var tokenBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new KeycloakAuthenticationException(response.StatusCode, tokenBody);
                }

                _token = JsonConvert.DeserializeObject<OidcToken>(tokenBody);

                if (_token == null || string.IsNullOrEmpty(_token.AccessToken))
                {
                    throw new KeycloakAuthenticationException(response.StatusCode, tokenBody);
                }

                SetCachedToken(_token);
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        private static HttpClient CreateDefaultHttpClient(KeycloakOptions options)
        {
            options.Validate();

            return new HttpClient
            {
                BaseAddress = new Uri(NormalizeServerUrl(options.ServerUrl)),
            };
        }

        private static string NormalizeServerUrl(string serverUrl)
        {
            return serverUrl.TrimEnd('/') + "/";
        }
    }
}
