using Keycloak.Constants.Enums;
using Keycloak.Entities;
using Keycloak.Entities.Keys;
using Keycloak.Helpers;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Keycloak.Validators
{
    public static class TokenValidator
    {
        public static ValidationStatusCode ValidateToken(Entities.TokenValidationParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Token) || !CheckFormat(parameters.Token))
            {
                return ValidationStatusCode.IncorrectFormat;
            }

            if (parameters.Keys?.Keys == null || !parameters.Keys.Keys.Any())
            {
                return ValidationStatusCode.MissingSigningKey;
            }

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(parameters.Token))
            {
                return ValidationStatusCode.IncorrectFormat;
            }

            var jwt = handler.ReadJwtToken(parameters.Token);

            if (parameters.CheckIssuer && jwt.Issuer != parameters.Issuer)
            {
                return ValidationStatusCode.IncorrectIssuer;
            }

            if (!HasMatchingSigningKey(jwt, parameters.Keys))
            {
                return ValidationStatusCode.MissingSigningKey;
            }

            if (parameters.CheckClient && !CheckClientClaim(jwt, parameters.Client))
            {
                return ValidationStatusCode.IncorrectClient;
            }

            if (parameters.CheckWebOrigins && !CheckWebOrigins(jwt, parameters.WebOrigins))
            {
                return ValidationStatusCode.InvalidWebOrigins;
            }

            try
            {
                var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = parameters.CheckIssuer,
                    ValidIssuer = parameters.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(parameters.ServerSkew),
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = GetSigningKeys(parameters.Keys),
                };

                handler.ValidateToken(parameters.Token, validationParameters, out SecurityToken _);

                return ValidationStatusCode.Ok;
            }
            catch (SecurityTokenExpiredException)
            {
                return ValidationStatusCode.Expired;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                return ValidationStatusCode.IncorrectIssuer;
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                return ValidationStatusCode.MissingSigningKey;
            }
            catch (SecurityTokenInvalidAlgorithmException)
            {
                return ValidationStatusCode.MissingAlgorithm;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return ValidationStatusCode.InvalidSignature;
            }
            catch (ArgumentException)
            {
                return ValidationStatusCode.IncorrectFormat;
            }
            catch (SecurityTokenException)
            {
                return ValidationStatusCode.InvalidSignature;
            }
        }

        public static bool CheckFormat(string token)
        {
            return token.Split('.').Length == 3;
        }

        public static bool CheckIssuer(Payload payload, string issuer)
        {
            return payload.Issuer == issuer;
        }

        public static bool CheckClient(Payload payload, string client)
        {
            return payload.AuthorizingParty == client;
        }

        public static bool CheckExpiration(Payload payload, int serverSkewSeconds = 0)
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(payload.Expiry);

            return expiry.AddSeconds(serverSkewSeconds) >= DateTimeOffset.UtcNow;
        }

        private static bool CheckClientClaim(JwtSecurityToken token, string client)
        {
            var azp = token.Claims.FirstOrDefault(claim => claim.Type == "azp")?.Value;
            return azp == client;
        }

        private static bool CheckWebOrigins(JwtSecurityToken token, IEnumerable<string> expectedOrigins)
        {
            if (expectedOrigins == null)
            {
                return true;
            }

            var allowedOrigins = new HashSet<string>(
                token.Claims.Where(claim => claim.Type == "allowed-origins").Select(claim => claim.Value),
                StringComparer.OrdinalIgnoreCase);

            return expectedOrigins.All(origin => allowedOrigins.Contains(origin));
        }

        private static bool HasMatchingSigningKey(JwtSecurityToken token, RealmKeys realmKeys)
        {
            var keyId = token.Header.Kid;

            return realmKeys.Keys.Any(key =>
                key.KeyType == "RSA" &&
                !string.IsNullOrEmpty(key.Modulus) &&
                !string.IsNullOrEmpty(key.Exponent) &&
                key.KeyId == keyId);
        }

        private static IEnumerable<SecurityKey> GetSigningKeys(RealmKeys realmKeys)
        {
            foreach (var key in realmKeys.Keys)
            {
                if (key.KeyType != "RSA" ||
                    string.IsNullOrEmpty(key.Modulus) ||
                    string.IsNullOrEmpty(key.Exponent))
                {
                    continue;
                }

                var rsa = RSA.Create();
                rsa.ImportParameters(new RSAParameters
                {
                    Modulus = JwtEncodingHelper.Base64UrlDecode(key.Modulus),
                    Exponent = JwtEncodingHelper.Base64UrlDecode(key.Exponent),
                });

                yield return new RsaSecurityKey(rsa) { KeyId = key.KeyId };
            }
        }
    }
}
