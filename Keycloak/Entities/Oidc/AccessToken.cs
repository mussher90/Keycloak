using Newtonsoft.Json;
using System.Collections.Generic;

namespace Keycloak.Entities
{
    public class AccessTokenPayload : Payload
    {
       
        [JsonProperty("auth_time")]
        public long AuthorizationTime { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("realm_access")]
        public RealmRoles RealmAccess { get; set; }

        [JsonProperty("resource_access")]
        public Dictionary<string, RealmRoles> ResourceAccess { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }

    public class RealmRoles
    {
        public List<string> Roles { get; set; }
    }
}
