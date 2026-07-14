using Keycloak.Constants.Enums;
using Keycloak.Entities.Users;
using Keycloak.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services.Users
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(UserRepresentation userRepresentation);

        Task UpdateUserAsync(UserRepresentation userRepresentation, string userId);

        Task ResetUserPasswordAsync(CredentialRepresentation credentialRepresentation, string userId);

        Task DeleteUserAsync(string userId);

        Task<List<Session>> GetUserSessionsAsync(string userId);

        Task SendEmailAsync(string userId, IEnumerable<RequiredActionsEnum> requiredActions);

        Task SendVerificationEmailAsync(string userId);
    }

    public class UserService : IUserService
    {
        private readonly IKeycloakClient _client;

        public UserService(IKeycloakClient client)
        {
            _client = client;
        }

        public Task<string> CreateUserAsync(UserRepresentation userRepresentation)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users";
            var byteContent = ApiHelper.FormatEntity(userRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Post, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAndGetCreatedResourceIdAsync(_client, message);
        }

        public Task UpdateUserAsync(UserRepresentation userRepresentation, string userId)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}";
            var byteContent = ApiHelper.FormatEntity(userRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message);
        }

        public Task ResetUserPasswordAsync(CredentialRepresentation credentialRepresentation, string userId)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/reset-password";
            var byteContent = ApiHelper.FormatEntity(credentialRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message);
        }

        public Task DeleteUserAsync(string userId)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}";
            var message = ApiHelper.ConstructRequest(HttpMethod.Delete, apiEndpoint);

            return KeycloakHttpHelper.SendAsync(_client, message);
        }

        public Task<List<Session>> GetUserSessionsAsync(string userId)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/sessions";
            var message = ApiHelper.ConstructRequest(HttpMethod.Get, apiEndpoint);

            return KeycloakHttpHelper.SendAndDeserializeAsync<List<Session>>(_client, message);
        }

        public Task SendEmailAsync(string userId, IEnumerable<RequiredActionsEnum> requiredActions)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/execute-actions-email";
            var actionsList = requiredActions.Select(action => action.ToKeycloakValue()).ToList();
            var byteContent = ApiHelper.FormatEntity(actionsList);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message);
        }

        public Task SendVerificationEmailAsync(string userId)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/send-verify-email";
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint);

            return KeycloakHttpHelper.SendAsync(_client, message);
        }
    }
}
