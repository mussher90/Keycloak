using System;
using System.Text;

namespace Keycloak.Helpers
{
    public static class JwtEncodingHelper
    {
        public static byte[] Base64UrlDecode(string encodedString)
        {
            encodedString = encodedString.Replace('_', '/').Replace('-', '+');

            while (encodedString.Length % 4 != 0)
            {
                encodedString += '=';
            }

            return Convert.FromBase64String(encodedString);
        }

        public static string Base64UrlEncode(string value)
        {
            return value.Replace('/', '_').Replace('+', '-');
        }
    }
}
