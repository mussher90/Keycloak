using Newtonsoft.Json;

namespace Keycloak.Entities.Users
{
    public class UserRepresentation
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("credentials")]

        public CredentialRepresentation[] Credentials; 
    }
}
