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
            if (!CheckFormat(parameters.BearerToken))
            {
                return false;
            }

            return CheckSignature(parameters.Keys, parameters.BearerToken);
        }

        public static bool CheckFormat(string token)
        {
            return token.Split('.').Length == 3;
        }

        public static bool CheckSignature(RealmKeys keys, string token)
        {
            string header;
            string payload;
            string signature;

            parseToken(token, out header, out payload, out signature);

            Header headerObj = JsonConvert.DeserializeObject<Header>(Encoding.ASCII.GetString(prepSignature(header)));

            var signingKey = keys.Keys
                            .Select(x => x)
                            .Where(x => x.KeyId == headerObj.KeyId)
                            .FirstOrDefault();

            var hashedPayload = CalculateHash(header, payload);
            var signatureBytes = prepSignature(signature);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters keyParams = new RSAParameters
                {
                    Modulus = prepSignature(signingKey.Modulus),
                    Exponent = prepSignature(signingKey.Exponent)
                };

                rsa.ImportParameters(keyParams);
                return rsa.VerifyHash(hashedPayload, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }

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


    }
}

