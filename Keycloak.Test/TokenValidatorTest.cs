using Keycloak.Validators;
using System;
using Xunit;

namespace Keycloak.Test
{
    public class TokenValidatorTest
    {
        [Fact]
        public void CheckFormat_AcceptsWellFormattedTokens()
        {
            Assert.True(TokenValidator.CheckFormat("header.payload.signature"));
        }

        [Fact]
        public void CheckFormat_RejectsBadlyFormattedTokens()
        {
            Assert.False(TokenValidator.CheckFormat("header.payload"));
        }

        [Fact]
        public void IsAccessTokenValid_RejectsExpiredToken()
        {
            var accessToken = CreateTestAccessToken(DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds());

            Assert.False(TokenValidator.IsAccessTokenValid(accessToken));
        }

        [Fact]
        public void IsAccessTokenValid_AcceptsUnexpiredToken()
        {
            var accessToken = CreateTestAccessToken(DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds());

            Assert.True(TokenValidator.IsAccessTokenValid(accessToken));
        }

        [Fact]
        public void IsAccessTokenValid_UsesUtc_AndRespectsServerSkew()
        {
            var accessToken = CreateTestAccessToken(DateTimeOffset.UtcNow.AddSeconds(-10).ToUnixTimeSeconds());

            Assert.False(TokenValidator.IsAccessTokenValid(accessToken, serverSkewSeconds: 0));
            Assert.True(TokenValidator.IsAccessTokenValid(accessToken, serverSkewSeconds: 30));
        }

        [Fact]
        public void IsAccessTokenValid_ReturnsFalse_ForMalformedToken()
        {
            Assert.False(TokenValidator.IsAccessTokenValid("not-a-jwt"));
        }

        [Fact]
        public void TryGetAccessTokenExpiry_ParsesJwtExpiry()
        {
            var expiryUnixSeconds = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
            var accessToken = CreateTestAccessToken(expiryUnixSeconds);

            Assert.True(TokenValidator.TryGetAccessTokenExpiry(accessToken, out var expiry));
            Assert.Equal(expiryUnixSeconds, expiry.ToUnixTimeSeconds());
        }

        private static string CreateTestAccessToken(long expiryUnixSeconds)
        {
            var header = Base64UrlEncode("{}");
            var payload = Base64UrlEncode($"{{\"exp\":{expiryUnixSeconds}}}");

            return $"{header}.{payload}.signature";
        }

        private static string Base64UrlEncode(string value)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
