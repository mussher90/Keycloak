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

        public Payload Payload { get; private set; }

        public string Signature { get; }

        public string EncodedHeader { 
            get => _encodedHeader; 
            set { 
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
            var decodedJsonHeader = Encoding.ASCII.GetString(TokenValidator.base64Decode(header));
            return JsonConvert.DeserializeObject<Header>(decodedJsonHeader);
        }

        public static Payload DecodeToken(string payload)
        {
            var decodedJsonToken = Encoding.ASCII.GetString(TokenValidator.base64Decode(payload));
            return JsonConvert.DeserializeObject<Payload>(decodedJsonToken);
        }
    }
}
