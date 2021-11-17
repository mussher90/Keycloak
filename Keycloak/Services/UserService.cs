using Keycloak.Constants;
using Keycloak.Constants.Enums;
using Keycloak.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Keycloak.Services
{
    public static class UserService
    {
        public static async Task<string> CreateUser(IKeycloakClient client, UserRepresentation userRepresentation)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users";

            var byteContent = ApiHelper.FormatEntity(userRepresentation);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = byteContent,
                RequestUri = new Uri(apiEndpoint, UriKind.Relative)
            };

            var response =  await client.Send(message, true).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return ExtractKeycloakUserId(response.Headers.Location);
            }

            return null;
        }

        public static async Task<HttpResponseMessage> UpdateUser(IKeycloakClient client, UserRepresentation userRepresentation, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}";

            var byteContent = ApiHelper.FormatEntity(userRepresentation);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                Content = byteContent,
                RequestUri = new Uri(apiEndpoint, UriKind.Relative)
            };

            return await client.Send(message, true).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DeleteUser(IKeycloakClient client, string userId)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}";

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(apiEndpoint, UriKind.Relative)
            };

            return await client.Send(message, true).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> SendEmail(IKeycloakClient client, string userId, IEnumerable<RequiredActionsEnum> requiredActions)
        {
            var apiEndpoint = $"auth/admin/realms/{client.Realm}/users/{userId}/execute-actions-email";

            var actionsList = from action in requiredActions
                              select RequireActionMapper(action);

            var byteContent = ApiHelper.FormatEntity(actionsList);

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                Content = byteContent,
                RequestUri = new Uri(apiEndpoint, UriKind.Relative)
            };

            return await client.Send(message, true).ConfigureAwait(false);
        }


        public static string ExtractKeycloakUserId(Uri userEndPoint)
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
