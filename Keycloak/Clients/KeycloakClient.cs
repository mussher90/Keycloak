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
using System.Threading.Tasks;

namespace Keycloak
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientSecret;
        private readonly string _clientId;
        private OidcToken _token;

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
            if (_token == null)
            {
                return false;
            }

            var jwt = new Jwt(_token.AccessToken);

            return TokenValidator.CheckExpiration(jwt.Payload);
        }

        private async Task SetToken()
        {
            if (!ValidToken())
            {
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
