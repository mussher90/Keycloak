using Keycloak.Entities;
using System.Security.Cryptography;
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

        [Fact]
        public void Should_Reject_Null_HashAlgorithm()
        {
            Header header = new Header
            {
                Algorithm = null
            };

            Assert.False(TokenValidator.CheckHashAlgorithm(header, out HashAlgorithmName? hashAlgorithm));
        }

        [Fact]
        public void Should_Reject_Incorrect_Client()
        {

        }

        [Fact]
        public void Should_Reject_Incorrect_Issuer()
        {

        }

        [Fact]
        public void Should_Reject_Expired_Token()
        {

        }
    }
}
