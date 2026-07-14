using System.Threading.Tasks;
using System.Net.Http;

namespace Keycloak
{
    public interface IKeycloakClient
    {
        string Realm { get; }

        string ServerUrl { get; }

        Task<HttpResponseMessage> Send(HttpRequestMessage message, bool requiresAccessToken = true);
    }
}
