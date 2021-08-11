using Keycloak.Entities;

namespace Keycloak.Clients
{
    public class UserClient : BaseClient
    {
        private readonly string EndPoint = "/users";

        public UserClient(string baseUrl)
        {
            BaseURl = baseUrl;
        }

        public string CreateUser(UserRepresentation userRepresentation)
        {
            var byteContent = FormatEntity(userRepresentation);

            var result = PostAsync(EndPoint, byteContent).Result;

            return result;
        }
    }
}
