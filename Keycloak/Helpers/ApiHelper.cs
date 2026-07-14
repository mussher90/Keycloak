using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Keycloak.Helpers
{
    public static class ApiHelper
    {
        public static ByteArrayContent FormatEntity(object keycloakEntity)
        {
            var entityJson = JsonConvert.SerializeObject(keycloakEntity);
            var buffer = Encoding.UTF8.GetBytes(entityJson);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }

        public static HttpRequestMessage ConstructRequest(HttpMethod method, string apiEndpoint, HttpContent content = null)
        {
            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(apiEndpoint, UriKind.Relative),
                Content = content,
            };
        }
    }
}
