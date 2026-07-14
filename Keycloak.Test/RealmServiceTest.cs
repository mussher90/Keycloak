using Keycloak.Entities.Realm;
using Keycloak.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Keycloak.Test
{
    public class RealmServiceTest
    {
        [Fact]
        public async Task GetRealmEventsAsync_BuildsQueryString()
        {
            var client = new FakeKeycloakClient(_ =>
                FakeKeycloakClient.JsonResponse(HttpStatusCode.OK, "[]"));
            var service = new RealmService(client);

            await service.GetRealmEventsAsync(new EventQuery
            {
                Client = "my-app",
                FromDate = new DateTime(2024, 1, 15),
                Max = 25,
            });

            Assert.Equal(
                "admin/realms/test-realm/events?client=my-app&dateFrom=2024-01-15&max=25",
                client.LastRequest.RequestUri.ToString());
        }
    }
}
