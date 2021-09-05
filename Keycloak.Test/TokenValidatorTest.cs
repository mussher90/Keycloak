using Keycloak.Entities;
using System.Security.Cryptography;
using Xunit;

namespace Keycloak.Test
{
    public class TokenValidatorTest
    {
        [Fact]
        public void Should_Accept_Well_Formatted_Tokens()
        {
            var token = "Blah.Blah.Blah";

            Assert.True(TokenValidator.CheckFormat(token));
        }

        [Fact]
        public void Should_Reject_Badly_Formatted_Tokens()
        {
            var token = "Blah.Blah";

            Assert.False(TokenValidator.CheckFormat(token));
        }

        [Fact]
        public void Should_Reject_Null_HashAlgorithm()
        {
            Header header = new Header
            {
                Algorithm = null
            };

            Assert.False(TokenValidator.CheckHashAlgorithm(header, out HashAlgorithmName? hashAlgorithm));
        }

        [Fact]
        public void Should_Reject_Incorrect_Client()
        {

        }

        [Fact]
        public void Should_Reject_Incorrect_Issuer()
        {

        }

        [Fact]
        public void Should_Reject_Expired_Token()
        {

        }

        [Fact]
        public void Should_Pass_Valid_Signature()
        {
            Key signingKey = new Key
            {
                Modulus = "2gwI-AjLH2lp7036yVxInnns0gxTxIDEdfWsbbhScZiQzX-Jqxsgnq0zde874uBFdANf1ufr9g0poMYp6EO6YJMQdUO0m2vDSswiqEW58FtMyjWD7iL7RdVQlitXuP4ab_wdlhP55cekyrdfgTyhRPHasbwRW2HfT3ZcA7M720oo3uP4X2a0YEsvIfa9XsXdTugzS4GjafbKTyf1U8HcKYdJJXgf0UU-RziXvio4HXd7hC1kSedgMusvT_0YKIWvmMCAIdsaZdgsF5iouU8ZvxrVx-guJlIgQ6d53lPfKJz-2KEcG-O_yIdmUre68kLdbtdeCbJdXnxT3_aPGDBNvw",
                Exponent = "AQAB"
            };

            Token token = new Token("eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJFUV94VHRfYjVNUzUyWmRCT3NwS2t5S3RSanRKbjctNHFtVmR6c0ZBSl9BIn0.eyJleHAiOjE2MzA4NzY5NzAsImlhdCI6MTYzMDg3NjY3MCwiYXV0aF90aW1lIjoxNjMwODc2NjcwLCJqdGkiOiJjN2NiMmQxNy1iNmFjLTQwNmEtOTJkNS0wNmFkNmI1YzE3ZTAiLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjgwODAvYXV0aC9yZWFsbXMvQmxvZyIsImF1ZCI6ImFjY291bnQiLCJzdWIiOiJlN2VlMGZmZS01ZGIxLTQwNjItOWQwNy05ODQyODA1YjFkNmMiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJibG9nRkUiLCJub25jZSI6IjA4MGRkNjViLTRhYTctNGUzMS1hYWYzLWJlNjEyZWJkYmNmZCIsInNlc3Npb25fc3RhdGUiOiJmZDY2ZTU5ZC02M2I1LTRlM2EtYWYyYS0zNTc0ZGIwMTcxNDgiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJdLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsib2ZmbGluZV9hY2Nlc3MiLCJ1bWFfYXV0aG9yaXphdGlvbiIsImRlZmF1bHQtcm9sZXMtYmxvZyJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIGVtYWlsIHByb2ZpbGUiLCJzaWQiOiJmZDY2ZTU5ZC02M2I1LTRlM2EtYWYyYS0zNTc0ZGIwMTcxNDgiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsIm5hbWUiOiJNYXR0aGV3IFVzc2hlciIsInByZWZlcnJlZF91c2VybmFtZSI6Im1hdHVzcyIsImdpdmVuX25hbWUiOiJNYXR0aGV3IiwiZmFtaWx5X25hbWUiOiJVc3NoZXIiLCJlbWFpbCI6Im11c3NoZXI5MEBnbWFpbC5jb20ifQ.rJ91d6-cj1fG-PTJSFn0I3tmaowvjJ-3pJyZL1PBnFRWHTTEPCa3bf9ifNIsjBkWwp30vty1U-0mTspEZZcSmG8WHqHj8dVnc8TrsdDsmFkpRlTIO3f3E8vI1xaIfx5hfvgMibo1yCTjVbNS48OStaWj2HTojQzEl3sinlBAfJSLEaYswBZ65xooaU5tOed-6NLghiPg-LkoNya7-V937AfmqzMmJbrjPNg2a6M0UavBfhrUoCQLM8DasKFC3UJ6tjZ1j1GEh6x713NY8Z71YKdBHM9Hd5yxm_b1o6PrZViN-k1I8Y-feS-ZVSKhMCX3HX_f1WUunEWbt4K4d6vVDQ");

            Assert.True(TokenValidator.CheckSignature(signingKey, token, HashAlgorithmName.SHA256));
        }
    }
}
