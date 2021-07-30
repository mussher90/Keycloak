using Keycloak.Entities;

namespace Keycloak
{
    public class TokenValidationParameters
    {
        public RealmKeys Keys { get; set; }

        public string BearerToken { get; set; }

        public string Issuer { get; set; }

        public string Client { get; set; }
    }
}
