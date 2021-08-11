using Keycloak.Clients;
using Keycloak.Entities;
using Newtonsoft.Json;

namespace Keycloak
{
    public class KeyClient : BaseClient
    {
        private readonly string EndPoint = "/protocol/openid-connect/certs";

        public KeyClient(string baseUrl)
        {
            BaseURl = baseUrl;
        }

        public RealmKeys GetKeys()
        {
            return JsonConvert.DeserializeObject<RealmKeys>(base.GetAsync(EndPoint).Result);
        }
    }
}
