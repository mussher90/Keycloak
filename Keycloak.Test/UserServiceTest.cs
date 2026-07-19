using Keycloak.Constants.Enums;
using Keycloak.Entities.Users;
using Keycloak.Services.Users;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Keycloak.Test
{
    public class UserServiceTest
    {
        [Fact]
        public async Task CreateUserAsync_ReturnsUserId_FromLocationHeader()
        {
            var client = new FakeKeycloakClient(_ =>
                FakeKeycloakClient.CreatedResponse("https://keycloak.test/admin/realms/test-realm/users/abc-123"));
            var service = new UserService(client);

            var userId = await service.CreateUserAsync(new UserRepresentation { UserName = "testuser" });

            Assert.Equal("abc-123", userId);
            Assert.Equal(HttpMethod.Post, client.LastRequest.Method);
            Assert.Equal("admin/realms/test-realm/users", client.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task CreateUserAsync_ThrowsKeycloakApiException_OnFailure()
        {
            var client = new FakeKeycloakClient(_ =>
                FakeKeycloakClient.JsonResponse(HttpStatusCode.Conflict, "{\"errorMessage\":\"User exists\"}"));
            var service = new UserService(client);

            var exception = await Assert.ThrowsAsync<KeycloakApiException>(
                () => service.CreateUserAsync(new UserRepresentation { UserName = "testuser" }));

            Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        }

        [Fact]
        public async Task DeleteUserAsync_SendsDeleteRequest()
        {
            var client = new FakeKeycloakClient(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
            var service = new UserService(client);

            await service.DeleteUserAsync("user-123");

            Assert.Equal(HttpMethod.Delete, client.LastRequest.Method);
            Assert.Equal("admin/realms/test-realm/users/user-123", client.LastRequest.RequestUri.ToString());
        }

        [Fact]
        public async Task DeleteUserAsync_PropagatesCancellationToken()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            var client = new FakeKeycloakClient(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
            var service = new UserService(client);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.DeleteUserAsync("user-123", cancellationTokenSource.Token));
        }

        [Fact]
        public async Task SendEmailAsync_SerializesRequiredActions()
        {
            string capturedBody = null;
            var client = new FakeKeycloakClient(request =>
            {
                capturedBody = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });
            var service = new UserService(client);

            await service.SendEmailAsync(
                "user-123",
                new[] { RequiredActionsEnum.UpdatePassword, RequiredActionsEnum.VerifyEmail });

            Assert.Contains("UPDATE_PASSWORD", capturedBody);
            Assert.Contains("VERIFY_EMAIL", capturedBody);
            Assert.Equal("admin/realms/test-realm/users/user-123/execute-actions-email", client.LastRequest.RequestUri.ToString());
        }
    }
}
