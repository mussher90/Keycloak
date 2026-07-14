using Keycloak.Entities.Keys;
using System.Collections.Generic;

namespace Keycloak.Entities
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
            get => _issuer;
            set
            {
                _issuer = value;
                CheckIssuer = true;
            }
        }

        public string Client
        {
            get => _client;
            set
            {
                _client = value;
                CheckClient = true;
            }
        }

        public IEnumerable<string> WebOrigins
        {
            get => _webOrigins;
            set
            {
                _webOrigins = value;
                CheckWebOrigins = true;
            }
        }

        public bool CheckIssuer { get; private set; }

        public bool CheckClient { get; private set; }

        public bool CheckWebOrigins { get; private set; }

        /// <summary>
        /// Clock skew in seconds applied when validating token expiry. Defaults to 0.
        /// </summary>
        public int ServerSkew { get; set; }
    }
}
