using System;
using System.Net;

namespace Keycloak
{
    public class KeycloakApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public string ResponseBody { get; }

        public Uri RequestUri { get; }

        public KeycloakApiException(HttpStatusCode statusCode, string responseBody, Uri requestUri)
            : base($"Keycloak API request failed. Status: {statusCode}. Uri: {requestUri}. Response: {responseBody}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            RequestUri = requestUri;
        }
    }
}
