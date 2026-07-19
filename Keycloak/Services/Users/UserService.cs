using Keycloak.Constants.Enums;
using Keycloak.Entities.Users;
using Keycloak.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Keycloak.Services.Users
{
    public interface IUserService
    {
        Task<string> CreateUserAsync(
            UserRepresentation userRepresentation,
            CancellationToken cancellationToken = default);

        Task UpdateUserAsync(
            UserRepresentation userRepresentation,
            string userId,
            CancellationToken cancellationToken = default);

        Task ResetUserPasswordAsync(
            CredentialRepresentation credentialRepresentation,
            string userId,
            CancellationToken cancellationToken = default);

        Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

        Task<List<Session>> GetUserSessionsAsync(string userId, CancellationToken cancellationToken = default);

        Task SendEmailAsync(
            string userId,
            IEnumerable<RequiredActionsEnum> requiredActions,
            CancellationToken cancellationToken = default);

        Task SendVerificationEmailAsync(string userId, CancellationToken cancellationToken = default);
    }

    public class UserService : IUserService
    {
        private readonly IKeycloakClient _client;

        public UserService(IKeycloakClient client)
        {
            _client = client;
        }

        public Task<string> CreateUserAsync(
            UserRepresentation userRepresentation,
            CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users";
            var byteContent = ApiHelper.FormatEntity(userRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Post, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAndGetCreatedResourceIdAsync(_client, message, cancellationToken: cancellationToken);
        }

        public Task UpdateUserAsync(
            UserRepresentation userRepresentation,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}";
            var byteContent = ApiHelper.FormatEntity(userRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message, cancellationToken: cancellationToken);
        }

        public Task ResetUserPasswordAsync(
            CredentialRepresentation credentialRepresentation,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/reset-password";
            var byteContent = ApiHelper.FormatEntity(credentialRepresentation);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message, cancellationToken: cancellationToken);
        }

        public Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}";
            var message = ApiHelper.ConstructRequest(HttpMethod.Delete, apiEndpoint);

            return KeycloakHttpHelper.SendAsync(_client, message, cancellationToken: cancellationToken);
        }

        public Task<List<Session>> GetUserSessionsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/sessions";
            var message = ApiHelper.ConstructRequest(HttpMethod.Get, apiEndpoint);

            return KeycloakHttpHelper.SendAndDeserializeAsync<List<Session>>(_client, message, cancellationToken: cancellationToken);
        }

        public Task SendEmailAsync(
            string userId,
            IEnumerable<RequiredActionsEnum> requiredActions,
            CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/execute-actions-email";
            var actionsList = requiredActions.Select(action => action.ToKeycloakValue()).ToList();
            var byteContent = ApiHelper.FormatEntity(actionsList);
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return KeycloakHttpHelper.SendAsync(_client, message, cancellationToken: cancellationToken);
        }

        public Task SendVerificationEmailAsync(string userId, CancellationToken cancellationToken = default)
        {
            var apiEndpoint = $"{KeycloakUriHelper.GetAdminRealmPath(_client.Realm)}/users/{userId}/send-verify-email";
            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint);

            return KeycloakHttpHelper.SendAsync(_client, message, cancellationToken: cancellationToken);
        }
    }
}
