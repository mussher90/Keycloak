using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak
{
    public interface IKeycloakClient
    {
        HttpClient Client { get; }

        string Realm { get;}

        Task<HttpResponseMessage> Send(HttpRequestMessage message, bool requiresAccessToken);
    }
}
