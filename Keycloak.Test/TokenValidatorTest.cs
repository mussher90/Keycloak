using Keycloak;
using Xunit;

namespace Keycloak.Test
{
    public class TokenValidatorTest
    {
        [Fact]
        public void Should_Accept_Well_Formatted_Tokens()
        {
            var token = "Blah.Blah.Blah";

            Assert.True(TokenValidator.CheckFormat(token));
        }

        [Fact]
        public void Should_Reject_Badly_Formatted_Tokens()
        {
            var token = "Blah.Blah";

            Assert.False(TokenValidator.CheckFormat(token));
        }
    }
}
