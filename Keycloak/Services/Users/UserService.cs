using Keycloak.Constants;
using Keycloak.Constants.Enums;
using Keycloak.Entities.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public static class UserService
    {
        public static async Task<string> CreateUserAsync(IKeycloakClient client, UserRepresentation userRepresentation)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users";

            var byteContent = ApiHelper.FormatEntity(userRepresentation);

            var message = ApiHelper.ConstructRequest(HttpMethod.Post, apiEndpoint, byteContent);

            var response =  await client.Send(message).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return ExtractKeycloakUserId(response.Headers.Location);
            }

            return null;
        }

        public static async Task<HttpResponseMessage> UpdateUserAsync(IKeycloakClient client, UserRepresentation userRepresentation, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}";

            var byteContent = ApiHelper.FormatEntity(userRepresentation);

            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return await client.Send(message).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> ResetUserPasswordAsync(IKeycloakClient client, CredentialRepresentation credentialRepresentation, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}/reset-password";

            var byteContent = ApiHelper.FormatEntity(credentialRepresentation);

            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return await client.Send(message).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DeleteUserAsync(IKeycloakClient client, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}";

            var message = ApiHelper.ConstructRequest(HttpMethod.Delete, apiEndpoint);

            return await client.Send(message).ConfigureAwait(false);
        }

        public static async Task<List<Session>> GetUserSessionsAsync(IKeycloakClient client, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}/sessions";

            var message = ApiHelper.ConstructRequest(HttpMethod.Get, apiEndpoint);

            var response = await client.Send(message).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var sesstionStream =  await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                using(var reader = new StreamReader(sesstionStream))
                {
                    return JsonConvert.DeserializeObject<List<Session>>(reader.ReadToEnd());
                }
            }

            return null;
        }

        public static async Task<HttpResponseMessage> SendEmailAsync(IKeycloakClient client, string userId, IEnumerable<RequiredActionsEnum> requiredActions)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}/execute-actions-email";

            var actionsList = (from action in requiredActions select RequireActionMapper(action)).ToList();

            var byteContent = ApiHelper.FormatEntity(actionsList);

            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint, byteContent);

            return await client.Send(message).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> SendVerificationEmailAsync(IKeycloakClient client, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}/send-verify-email";

            var message = ApiHelper.ConstructRequest(HttpMethod.Put, apiEndpoint);

            return await client.Send(message).ConfigureAwait(false);
        }

        private static string ExtractKeycloakUserId(Uri userEndPoint)
        {
            return userEndPoint.Segments.Last();
        }

        private static string RequireActionMapper(RequiredActionsEnum requiredActions)
        {
            var action = "";
            switch (requiredActions)
            {
                case RequiredActionsEnum.UpdatePassword:
                    action = RequiredActions.UpdatePassword;
                    break;
                case RequiredActionsEnum.VerifyEmail:
                    action = RequiredActions.VerifyEmail;
                    break;
                case RequiredActionsEnum.UpdateProfile:
                    action = RequiredActions.UpdateProfile;
                    break;
                case RequiredActionsEnum.ConfigureOTP:
                    action = RequiredActions.ConfigureOTP;
                    break;
                case RequiredActionsEnum.TermsAndConditions:
                    action = RequiredActions.TermsAndConditions;
                    break;
            }

            return action;
        }

    }
}
