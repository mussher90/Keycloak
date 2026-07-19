using Keycloak.Entities.Keys;
using Keycloak.Helpers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak.Services.Keys
{
    public interface IKeyService
    {
        Task<RealmKeys> GetKeysAsync(CancellationToken cancellationToken = default);
    }

    public class KeyService : IKeyService
    {
        private readonly IKeycloakClient _client;

        public KeyService(IKeycloakClient client)
        {
            _client = client;
        }

        public Task<RealmKeys> GetKeysAsync(CancellationToken cancellationToken = default)
        {
            var keyEndpoint = KeycloakUriHelper.GetCertsEndpoint(_client.Realm);
            var message = ApiHelper.ConstructRequest(HttpMethod.Get, keyEndpoint);

            return KeycloakHttpHelper.SendAndDeserializeAsync<RealmKeys>(
                _client,
                message,
                requiresAccessToken: false,
                cancellationToken: cancellationToken);
        }
    }
}
