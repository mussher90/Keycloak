using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Keycloak.Entities.Realm
{
    public class EventQuery
    {
        [JsonProperty("client")]
        public string Client { get; set; }

        [JsonProperty("dateFrom")]
        public DateTime? FromDate { get; set; }

        [JsonProperty("dateTo")]
        public DateTime? ToDate { get; set; }

        [JsonProperty("first")]
        public int? First { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("max")]
        public int? Max { get; set; }

        [JsonProperty("type")]
        public string[] Type { get; set; }

        [JsonProperty("user")]
        public string KeycloakUserId { get; set; }

        public string GetQueryString()
        {
            var parameters = new List<string>();

            if (Client != null)
            {
                parameters.Add($"client={Uri.EscapeDataString(Client)}");
            }

            if (FromDate != null)
            {
                parameters.Add($"dateFrom={FromDate.Value:yyyy-MM-dd}");
            }

            if (ToDate != null)
            {
                parameters.Add($"dateTo={ToDate.Value:yyyy-MM-dd}");
            }

            if (First != null)
            {
                parameters.Add($"first={First.Value}");
            }

            if (IpAddress != null)
            {
                parameters.Add($"ipAddress={Uri.EscapeDataString(IpAddress)}");
            }

            if (Max != null)
            {
                parameters.Add($"max={Max.Value}");
            }

            if (Type != null)
            {
                foreach (var eventType in Type)
                {
                    if (eventType != null)
                    {
                        parameters.Add($"type={Uri.EscapeDataString(eventType)}");
                    }
                }
            }

            if (KeycloakUserId != null)
            {
                parameters.Add($"user={Uri.EscapeDataString(KeycloakUserId)}");
            }

            if (parameters.Count == 0)
            {
                return string.Empty;
            }

            return "?" + string.Join("&", parameters);
        }
    }
}
