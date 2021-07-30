using Keycloak.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Keycloak
{
    public static class TokenValidator
    {        
         public static bool ValidateToken(TokenValidationParameters parameters)
        {
            string header = "";
            string payload = "";
            string signature = "";

            parseToken(parameters.BearerToken, out header, out payload, out signature);

            var hashedPayload = CalculateHash(header, payload);
            var signatureBytes = prepSignature(signature);

            return CheckSignature(parameters.Keys, hashedPayload, signatureBytes);
        }

        private static void parseToken(string accessToken, out string header, out string payload, out string signature)
        {

            var accessTokenArray = accessToken.Split('.');

            header = accessTokenArray[0];

            payload = accessTokenArray[1];

            signature = accessTokenArray[2];
        }

        private static byte[] prepSignature(string signature)
        {

            signature = signature.Replace('_', '/').Replace('-', '+');

            while (signature.Length % 4 != 0)
            {
                signature += '=';
            }

            return Convert.FromBase64String(signature);
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

        private static bool CheckSignature(RealmKeys keys, byte[] payload, byte[] signature)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters keyParams = new RSAParameters
                {
                    Modulus = prepSignature(keys.Keys[0].Modulus),
                    Exponent = prepSignature(keys.Keys[0].Exponent)
                };

                rsa.ImportParameters(keyParams);
                return rsa.VerifyHash(payload, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }

        }
    }
}

