using Keycloak.Entities.Keys;
using System.Collections.Generic;

namespace Keycloak
{
    public class TokenValidationParameters
    {
        private string _issuer;
        private string _client;
        private IEnumerable<string> _webOrigins;

        public RealmKeys Keys { get; set; }

        public string Token { get; set; }

        public string Issuer
        {
            get { return _issuer; }
            set
            {
                _issuer = value;
                CheckIssuer = true;
            }
        }

        public string Client
        {
            get { return _client; }
            set
            {
                _client = value;
                CheckClient = true;
            }
        }

        public IEnumerable<string> WebOrigins
        {
            get { return _webOrigins; }
            set
            {
                _webOrigins = value;
                CheckWebOrigins = true;
            }
        }

        public bool CheckIssuer { get; private set; }
        public bool CheckClient { get; private set; }
        public bool CheckWebOrigins { get; private set; }
        public int ServerSkew { get; set; }
    }
}
