using System;
using System.Net;

namespace Keycloak
{
    public class KeycloakAuthenticationException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string ResponseBody { get; }

        public KeycloakAuthenticationException(HttpStatusCode statusCode, string responseBody)
            : base($"Failed to acquire Keycloak access token. Status: {statusCode}. Response: {responseBody}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
