using Keycloak.Helpers;
using Xunit;

namespace Keycloak.Test
{
    public class KeycloakUriHelperTest
    {
        [Fact]
        public void GetIssuer_BuildsRealmIssuerUrl()
        {
            var issuer = KeycloakUriHelper.GetIssuer("https://keycloak.example.com", "myrealm");

            Assert.Equal("https://keycloak.example.com/realms/myrealm", issuer);
        }

        [Fact]
        public void GetIssuer_TrimsTrailingSlashFromServerUrl()
        {
            var issuer = KeycloakUriHelper.GetIssuer("https://keycloak.example.com/", "myrealm");

            Assert.Equal("https://keycloak.example.com/realms/myrealm", issuer);
        }

        [Fact]
        public void GetTokenEndpoint_UsesModernPath()
        {
            Assert.Equal("realms/myrealm/protocol/openid-connect/token", KeycloakUriHelper.GetTokenEndpoint("myrealm"));
        }

        [Fact]
        public void GetCertsEndpoint_UsesModernPath()
        {
            Assert.Equal("realms/myrealm/protocol/openid-connect/certs", KeycloakUriHelper.GetCertsEndpoint("myrealm"));
        }

        [Fact]
        public void GetAdminRealmPath_UsesModernPath()
        {
            Assert.Equal("admin/realms/myrealm", KeycloakUriHelper.GetAdminRealmPath("myrealm"));
        }
    }
}
