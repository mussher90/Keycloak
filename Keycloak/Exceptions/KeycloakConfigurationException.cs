using System;

namespace Keycloak
{
    public class KeycloakConfigurationException : Exception
    {
        public KeycloakConfigurationException(string message) : base(message)
        {
        }
    }
}
