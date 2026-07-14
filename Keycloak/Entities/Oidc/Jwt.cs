using Keycloak.Entities;
using Keycloak.Helpers;
using Newtonsoft.Json;
using System.Text;

namespace Keycloak.Entities
{
    public class Jwt
    {
        private string _encodedHeader;
        private string _encodedPayload;

        public Jwt(string token)
        {
            var accessTokenArray = token.Split('.');

            EncodedHeader = accessTokenArray[0];
            EncodedPayload = accessTokenArray[1];
            Signature = accessTokenArray[2];
        }

        public Header Header { get; private set; }

        public AccessTokenPayload Payload { get; private set; }

        public string Signature { get; }

        public string EncodedHeader
        {
            get => _encodedHeader;
            set
            {
                Header = DecodeHeader(value);
                _encodedHeader = value;
            }
        }

        public string EncodedPayload
        {
            get => _encodedPayload;
            set
            {
                Payload = DecodeToken(value);
                _encodedPayload = value;
            }
        }

        public static Header DecodeHeader(string header)
        {
            var decodedJsonHeader = Encoding.UTF8.GetString(JwtEncodingHelper.Base64UrlDecode(header));
            return JsonConvert.DeserializeObject<Header>(decodedJsonHeader);
        }

        public static AccessTokenPayload DecodeToken(string payload)
        {
            var decodedJsonToken = Encoding.UTF8.GetString(JwtEncodingHelper.Base64UrlDecode(payload));
            return JsonConvert.DeserializeObject<AccessTokenPayload>(decodedJsonToken);
        }
    }
}
