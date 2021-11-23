using System.Threading.Tasks;
using Keycloak.Enums;
using Keycloak.Services.Keys;
using Keycloak.Services.Keys.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Keycloak.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly IKeycloakClient _client;

        public TokenMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache cache, IKeycloakClient client)
        {
            _configuration = configuration;
            _next = next;
            _cache = cache;
            _client = client;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string accessToken = null;
            bool isValid = false;
            
            var authorizationHeaders = httpContext.Request.Headers["Authorization"];
            var authenticationURI = _configuration["KeycloakServer"];

            var realmKeys = GetKeys().Result;

            if(authorizationHeaders.Count == 1)
            {
                accessToken = authorizationHeaders.ToString().Split(' ')[1];

                var parameters = new TokenValidationParameters
                {
                    Token = accessToken,
                    Keys = realmKeys,
                    Issuer = authenticationURI,
                };

                isValid = TokenValidator.ValidateToken(parameters).Equals(ValidationStatusCode.Ok);
            }

            if (!isValid)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return httpContext.Response.WriteAsync("Unauthorized");
            }

            return _next(httpContext);
        }
        private async Task<RealmKeys> GetKeys()
        {
            if(_cache.Get("RealmKeys") != null)
            {
                return (RealmKeys)_cache.Get("RealmKeys");
            }

            var realmKeys = await KeyService.GetKeysAsync(_client);

            _cache.Set("RealmKeys", realmKeys);

            return realmKeys;
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JWTValidatorExtensions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}
