using Keycloak.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public class TokenService : BaseService
    {
        private readonly string EndPoint = "/protocol/openid-connect/token";

        public TokenService(HttpClient client, Uri baseUri) : base(client, baseUri)
        {
            _client = client;
            _baseUri = baseUri;
        }

        public async Task<OidcToken> GetToken(Dictionary<string, string> grant)
        {
            var byteContent = new FormUrlEncodedContent(grant);

            var response = await _client.PostAsync(_baseUri + EndPoint, byteContent);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<OidcToken>(token);
            }

            return null;
        }
    }
}
