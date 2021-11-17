using Keycloak.Entities;
using Microsoft.Extensions.Configuration;
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
        private readonly string ClientSecret;
        private readonly string ClientId;
        private OidcToken Token;

        public KeycloakClient(IConfiguration configuration)
        {
            Client = new HttpClient { BaseAddress = new Uri(configuration["KeycloakServer"]) };
            Realm = configuration["Realm"];
            ClientId = configuration["ClientId"];
            ClientSecret = configuration["ClientSecret"];
        }

        public KeycloakClient(Uri host, string realm, string clientID, string clientSecret)
        {
            Client = new HttpClient { BaseAddress = host };
            Realm = realm;
            ClientId = clientID;
            ClientSecret = clientSecret;
        }

        public HttpClient Client { get; }
        public string Realm { get; }

        public async Task<HttpResponseMessage> Send(HttpRequestMessage message, bool requiresAccessToken)
        {
            if (requiresAccessToken)
            {
                await SetToken().ConfigureAwait(false);

                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            }

            return await Client.SendAsync(message).ConfigureAwait(false);
        }

        private bool ValidToken()
        {
            if (Token == null)
            {
                return false;
            }

            var _jwt = new Jwt(Token.AccessToken);

            return TokenValidator.CheckExpiration(_jwt.Payload);
        }

        private async Task SetToken()
        {
            if (!ValidToken())
            {
                var apiEndpoint = $"auth/realms/{Realm}/protocol/openid-connect/token";

                var grant = new Dictionary<string, string>();

                grant.Add("grant_type", "client_credentials");
                grant.Add("client_id", ClientId);
                grant.Add("client_secret", ClientSecret);

                var content = new FormUrlEncodedContent(grant);

                var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = content,
                    RequestUri = new Uri(apiEndpoint, UriKind.Relative)
                };

                var response = await Send(message, false).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();

                    Token = JsonConvert.DeserializeObject<OidcToken>(token);
                }
            }
        }
    }
}
