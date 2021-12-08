using Newtonsoft.Json;
using System;

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
            var queryString = "";

            if(Client != null)
            {
                queryString += $"?client={Client}";
            }

            if(FromDate != null)
            {
                var fromDate = (DateTime)FromDate;
                if (queryString != "")
                {
                    queryString += $"?dateFrom={fromDate:yyyy-MM-dd}";
                }
                else
                {
                    queryString += $"&dateFrom={fromDate:yyyy-MM-dd}";
                }
            }

            if (ToDate != null)
            {
                var toDate = (DateTime)ToDate;
                if (queryString != "")
                {
                    queryString += $"?dateTo={toDate:yyyy-MM-dd}";
                }
                else
                {
                    queryString += $"&dateTo={toDate:yyyy-MM-dd}";
                }
            }

            if (First != null)
            {
                if (queryString != "")
                {
                    queryString += $"?first={First}";
                }
                else
                {
                    queryString += $"&first={First}";
                }
            }

            if (IpAddress != null)
            {
                if (queryString != "")
                {
                    queryString += $"?ipAddress={IpAddress}";
                }
                else
                {
                    queryString += $"&ipAddress={IpAddress}";
                }
            }

            if (Max != null)
            {
                if (queryString != "")
                {
                    queryString += $"?max={Max}";
                }
                else
                {
                    queryString += $"&max={Max}";
                }
            }

            if (Type != null)
            {
                if (queryString != "")
                {
                    queryString += $"?type={Type}";
                }
                else
                {
                    queryString += $"&type={Type}";
                }
            }

            if (KeycloakUserId != null)
            {
                if (queryString != "")
                {
                    queryString += $"?user={KeycloakUserId}";
                }
                else
                {
                    queryString += $"&user={KeycloakUserId}";
                }
            }

            return queryString;
        }
    }
}
