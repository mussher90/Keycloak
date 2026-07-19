using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak
{
    public interface IKeycloakClient
    {
        string Realm { get; }

        string ServerUrl { get; }

        Task<HttpResponseMessage> Send(
            HttpRequestMessage message,
            bool requiresAccessToken = true,
            CancellationToken cancellationToken = default);
    }
}
