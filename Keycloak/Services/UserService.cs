using Keycloak.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public class UserService : BaseService
    {
        private readonly string EndPoint = "/users";

        public UserService(HttpClient client, Uri baseUri):base(client, baseUri)
        {
            _client = client;
            _baseUri = baseUri;
        }

        public async Task<HttpResponseMessage> CreateUser(UserRepresentation userRepresentation)
        {
            var byteContent = FormatEntity(userRepresentation);

            return await _client.PostAsync(_baseUri + EndPoint, byteContent);
        }

        public async Task<HttpResponseMessage> SendEmail(Uri userEndPoint, IEnumerable<string> requiredActions)
        {
            var byteContent = FormatEntity(requiredActions);

            return await _client.PutAsync(userEndPoint, byteContent);
        }

        public string ExtractKeycloakUserId(Uri userEndPoint)
        {
            return userEndPoint.Segments.Last();
        }

    }
}
