using Keycloak.Entities;
using Keycloak.Enums;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Keycloak
{
    public static class TokenValidator
    {        
         public static ValidationStatusCode ValidateToken(TokenValidationParameters parameters)
        {
            var token = parameters.Token;
            var keys = parameters.Keys;
            var client = parameters.Client;
            var checkClient = parameters.CheckClient;
            var issuer = parameters.Issuer;
            var checkIssuer = parameters.CheckIssuer;
            var webOrigins = parameters.WebOrigins;
            var checkWebOrigins = parameters.CheckWebOrigins;
            Jwt tokenObj;

            if (!CheckFormat(token))
            {
                return ValidationStatusCode.IncorrectFormat;
            }

            tokenObj = new Jwt(token);

            if (!CheckHashAlgorithm(tokenObj.Header, out HashAlgorithmName? hashAlgorithm))
            {
                return ValidationStatusCode.MissingAlgorithm;
            }

            if(checkIssuer && !CheckIssuer(tokenObj.Payload, issuer))
            {
                return ValidationStatusCode.IncorrectIssuer;
            }

            if (checkClient && !CheckClient(tokenObj.Payload, client))
            {
                return ValidationStatusCode.IncorrectClient;
            }

            if (!CheckExpiration(tokenObj.Payload))
            {
                return ValidationStatusCode.Expired;
            }

            var signingKey = keys.Keys
                            .Select(x => x)
                            .Where(x => x.KeyId == tokenObj.Header.KeyId)
                            .FirstOrDefault();

            return CheckSignature(signingKey, tokenObj, (HashAlgorithmName)hashAlgorithm) ? ValidationStatusCode.Ok : ValidationStatusCode.InvalidSignature;
        }

        public static bool CheckFormat(string token)
        {
            return token.Split('.').Length == 3;
        }

        public static bool CheckHashAlgorithm(Header header, out HashAlgorithmName? hashAlgorithm)
        {
            hashAlgorithm = GetHashAlgorithm(header);

            return hashAlgorithm != null;
        }

        public static bool CheckIssuer(Payload payload, string Issuer)
        {
            return payload.Issuer == Issuer;
        }

        public static bool CheckClient(Payload payload, string Client)
        {
            return payload.AuthorizingParty == Client;
        }

        public static bool CheckExpiration(Payload payload)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(payload.Expiry).ToLocalTime();

            return dateTime >= DateTime.Now;
        }

        public static bool CheckSignature(Key signingKey, Jwt token, HashAlgorithmName algorithmName)
        {
            var hashedPayload = CalculateHash(token.EncodedHeader, token.EncodedPayload);
            var signatureBytes = base64Decode(token.Signature);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters keyParams = new RSAParameters
                {
                    Modulus = base64Decode(signingKey.Modulus),
                    Exponent = base64Decode(signingKey.Exponent)
                };

                rsa.ImportParameters(keyParams);
                return rsa.VerifyHash(hashedPayload, signatureBytes, algorithmName, RSASignaturePadding.Pkcs1);
            }
        }

        public static byte[] base64Decode(string encodedString) 
        {

            encodedString = encodedString.Replace('_', '/').Replace('-', '+');

            while (encodedString.Length % 4 != 0)
            {
                encodedString += '=';
            }

            return Convert.FromBase64String(encodedString);
        }

        public static string urlEncode(string encodedString)
        {
            return encodedString.Replace('/', '_').Replace('+', '-');
        }

        private static byte[] CalculateHash(string header, string payload)
        {
            var byteArray = Encoding.UTF8.GetBytes(header + "." + payload);
            byte[] hashedResult = null;

            //shouldn't assume SHA256
            using (SHA256 hasher = SHA256.Create())
            {
                hashedResult = hasher.ComputeHash(byteArray);
            }
            return hashedResult;
        }

        private static HashAlgorithmName? GetHashAlgorithm(Header header)
        {
            switch (header.Algorithm)
            {
                case "RS256":
                    return HashAlgorithmName.SHA256;
                default:
                    return null;
            }
        }

    }
}

