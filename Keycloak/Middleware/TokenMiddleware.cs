using System.Threading.Tasks;
using Keycloak.Configuration;
using Keycloak.Constants.Enums;
using Keycloak.Entities;
using Keycloak.Entities.Keys;
using Keycloak.Services.Keys;
using Keycloak.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Keycloak.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IKeyService _keyService;
        private readonly KeycloakOptions _options;

        public TokenMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            IKeyService keyService,
            KeycloakOptions options)
        {
            _next = next;
            _cache = cache;
            _keyService = keyService;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var isValid = false;

            if (TryExtractBearerToken(httpContext, out var accessToken))
            {
                var realmKeys = await GetKeys().ConfigureAwait(false);
                var parameters = BuildValidationParameters(accessToken, realmKeys);

                isValid = TokenValidator.ValidateToken(parameters).Equals(ValidationStatusCode.Ok);
            }

            if (!isValid)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Unauthorized").ConfigureAwait(false);
                return;
            }

            await _next(httpContext).ConfigureAwait(false);
        }

        internal TokenValidationParameters BuildValidationParameters(string accessToken, RealmKeys realmKeys)
        {
            var parameters = new TokenValidationParameters
            {
                Token = accessToken,
                Keys = realmKeys,
                Issuer = _options.Authority,
                ServerSkew = _options.ServerSkew,
            };

            if (_options.ValidateClientId)
            {
                parameters.Client = _options.ClientId;
            }

            return parameters;
        }

        private static bool TryExtractBearerToken(HttpContext httpContext, out string accessToken)
        {
            accessToken = null;

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return false;
            }

            var headerValue = authorizationHeader.ToString();

            if (!headerValue.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            accessToken = headerValue.Substring("Bearer ".Length).Trim();

            return !string.IsNullOrEmpty(accessToken);
        }

        private async Task<RealmKeys> GetKeys()
        {
            var cacheKey = RealmKeysCache.GetCacheKey(_options.Realm);

            if (_cache.TryGetValue(cacheKey, out RealmKeys cachedKeys))
            {
                return cachedKeys;
            }

            var realmKeys = await _keyService.GetKeysAsync().ConfigureAwait(false);

            _cache.Set(cacheKey, realmKeys, RealmKeysCache.CreateEntryOptions(_options));

            return realmKeys;
        }
    }

    public static class JWTValidatorExtensions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }

        public static IApplicationBuilder UseKeycloakTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}
