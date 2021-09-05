using Keycloak.Entities;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Keycloak
{
    public static class TokenValidator
    {        
         public static bool ValidateToken(TokenValidationParameters parameters)
        {
            var token = parameters.BearerToken;
            var keys = parameters.Keys;
            var client = parameters.Client;
            var issuer = parameters.Issuer;
            Token tokenObj;

            if (!CheckFormat(token))
            {
                return false;
            }

            tokenObj = new Token(token);

            if (!CheckHashAlgorithm(tokenObj.Header, out HashAlgorithmName? hashAlgorithm))
            {
                return false;
            }

            if(!CheckIssuer(tokenObj.Payload, issuer))
            {
                return false;
            }

            if (!CheckClient(tokenObj.Payload, client))
            {
                return false;
            }

            if (!CheckExpiration(tokenObj.Payload))
            {
                return false;
            }

            var signingKey = keys.Keys
                            .Select(x => x)
                            .Where(x => x.KeyId == tokenObj.Header.KeyId)
                            .FirstOrDefault();

            return CheckSignature(signingKey, tokenObj, (HashAlgorithmName)hashAlgorithm);
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

        public static bool CheckIssuer(AccessToken token, string Issuer)
        {
            return token.Issuer == Issuer;
        }

        public static bool CheckClient(AccessToken token, string Client)
        {
            return token.AuthorizingParty == Client;
        }

        public static bool CheckExpiration(AccessToken token)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(token.Expiry).ToLocalTime();

            return dateTime < DateTime.Now;
        }

        public static bool CheckSignature(Key signingKey, Token token, HashAlgorithmName algorithmName)
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

        private static byte[] CalculateHash(string header, string payload)
        {
            var byteArray = Encoding.UTF8.GetBytes(header + "." + payload);
            byte[] hashedResult = null;
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

