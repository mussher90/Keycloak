using Keycloak.Entities.Realm;
using Keycloak.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public interface IRealmService
    {
        Task<List<Event>> GetRealmEventsAsync(EventQuery eventQuery, CancellationToken cancellationToken = default);
    }

    public class RealmService : IRealmService
    {
        private readonly IKeycloakClient _client;

        public RealmService(IKeycloakClient client)
        {
            _client = client;
        }

        public Task<List<Event>> GetRealmEventsAsync(EventQuery eventQuery, CancellationToken cancellationToken = default)
        {
            var queryParameters = eventQuery.GetQueryString();
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/events{queryParameters}";
            var message = ApiHelper.ConstructRequest(HttpMethod.Get, apiEndpoint);

            return KeycloakHttpHelper.SendAndDeserializeAsync<List<Event>>(_client, message, cancellationToken: cancellationToken);
        }
    }
}
