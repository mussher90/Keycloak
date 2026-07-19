using System;
using System.IdentityModel.Tokens.Jwt;

namespace Keycloak.Validators
{
    public static class TokenValidator
    {
        public static bool CheckFormat(string token)
        {
            return token.Split('.').Length == 3;
        }

        public static bool IsAccessTokenValid(string accessToken, int serverSkewSeconds = 0)
        {
            if (!TryGetAccessTokenExpiry(accessToken, out var expiry))
            {
                return false;
            }

            return IsExpiryValid(expiry, serverSkewSeconds);
        }

        public static bool TryGetAccessTokenExpiry(string accessToken, out DateTimeOffset expiry)
        {
            expiry = default;

            if (string.IsNullOrWhiteSpace(accessToken) || !CheckFormat(accessToken))
            {
                return false;
            }

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                return false;
            }

            var jwt = handler.ReadJwtToken(accessToken);
            expiry = ToUtcDateTimeOffset(jwt.ValidTo);
            return true;
        }

        private static bool IsExpiryValid(DateTimeOffset expiry, int serverSkewSeconds)
        {
            return expiry.AddSeconds(serverSkewSeconds) >= DateTimeOffset.UtcNow;
        }

        private static DateTimeOffset ToUtcDateTimeOffset(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
            }

            return new DateTimeOffset(dateTime.ToUniversalTime());
        }
    }
}
