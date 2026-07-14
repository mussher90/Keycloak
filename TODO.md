# Keycloak Library — TODO

Improvement backlog for future work. Items are roughly ordered by impact. Check off items as they are completed.

---

## High impact

- [ ] **Thread-safe token refresh in `KeycloakClient`**
  - `SetToken()` has no synchronization; concurrent requests when a token expires can race and fetch multiple tokens.
  - Consider a `SemaphoreSlim(1, 1)` with double-checked locking after acquire.
  - File: `Keycloak/Clients/KeycloakClient.cs`

- [ ] **Unify token expiry checking**
  - Middleware uses IdentityModel with `ClockSkew`; `KeycloakClient.ValidToken()` still uses the custom `Jwt` entity and `CheckExpiration` without skew.
  - Admin API token refresh and middleware validation can disagree on whether a token is expired.
  - Prefer one path — e.g. read `exp` via `JwtSecurityTokenHandler` and apply the same skew from options.
  - Files: `Keycloak/Clients/KeycloakClient.cs`, `Keycloak/Validators/TokenValidator.cs`

- [ ] **Add `CancellationToken` support**
  - No HTTP calls accept cancellation today.
  - Add `CancellationToken cancellationToken = default` to `IKeycloakClient.Send`, service methods, and middleware.
  - Files: `Keycloak/Clients/IKeycloakClient.cs`, `Keycloak/Clients/KeycloakClient.cs`, service interfaces/implementations, `Keycloak/Middleware/TokenMiddleware.cs`

- [ ] **Populate `HttpContext.User` in middleware**
  - Middleware validates tokens but does not attach claims to the request context.
  - Downstream code cannot use `[Authorize]`, `User.Identity`, or policy-based auth without re-parsing the token.
  - Consider setting `httpContext.User` from validated JWT claims, or document integration with `AddAuthentication().AddJwtBearer()`.
  - File: `Keycloak/Middleware/TokenMiddleware.cs`

---

## Middleware and auth UX

- [ ] **Support optional / mixed auth routes**
  - Missing `Authorization` header currently returns 401; no way to protect only some routes without external wrapping.
  - Options: auth handler registration instead of global middleware, `RequireAuthentication = false` option, or endpoint metadata (`[AllowAnonymous]`).
  - File: `Keycloak/Middleware/TokenMiddleware.cs`

- [ ] **JWKS cache refresh on unknown `kid`**
  - If Keycloak rotates keys, cached JWKS can reject valid tokens until cache expiry.
  - On `MissingSigningKey`, invalidate cache, refetch once, and retry validation.
  - Files: `Keycloak/Middleware/TokenMiddleware.cs`, `Keycloak/Middleware/RealmKeysCache.cs`

- [ ] **Improve 401 responses**
  - Plain `"Unauthorized"` with no `WWW-Authenticate` header makes debugging harder for API clients.
  - Consider a structured JSON error body and/or standard auth headers.
  - File: `Keycloak/Middleware/TokenMiddleware.cs`

---

## API design and ergonomics

- [ ] **Add `AddKeycloak` overloads**
  - Currently only `AddKeycloak(IServiceCollection, IConfiguration)`.
  - Consider: `AddKeycloak(services, Action<KeycloakOptions> configure)` and `AddKeycloak(services, KeycloakOptions options)`.
  - Consider `services.AddOptions<KeycloakOptions>().BindConfiguration(...).ValidateOnStart()` instead of manual bind + validate.
  - File: `Keycloak/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Register middleware helpers together**
  - Consumers must remember: `AddMemoryCache()`, `AddKeycloak()`, `UseKeycloakTokenValidation()`.
  - Consider having `AddKeycloak` optionally register `IMemoryCache`, or provide `AddKeycloakAuth()` that wires JWT bearer.
  - Files: `Keycloak/Extensions/ServiceCollectionExtensions.cs`, `README.md`

- [ ] **Expand admin API surface**
  - Currently covered: users (CRUD-ish), password reset, emails, sessions, events, keys.
  - Common missing operations: get user by ID, search users, role/group management, client/realm configuration.
  - Define scope explicitly in README — "admin user management" vs "full Keycloak admin SDK".

- [ ] **Null-safe deserialization**
  - `KeycloakHttpHelper.SendAndDeserializeAsync` can return `null` without a clear error.
  - Throw `KeycloakApiException` when a successful response deserializes to null for non-nullable expected types.
  - File: `Keycloak/Helpers/KeycloakHttpHelper.cs`

---

## Configuration and resilience

- [ ] **Configurable HTTP timeouts and resilience**
  - `AddHttpClient` currently configures base address only.
  - Consider options for `Timeout`, retry on 429/503 (Polly / `Microsoft.Extensions.Http.Resilience`), and optional certificate pinning.
  - Files: `Keycloak/Configuration/KeycloakOptions.cs`, `Keycloak/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Support `IOptionsMonitor` / hot reload**
  - Options are bound once at startup into a singleton; config reload won't pick up rotated secrets.
  - Consider `IOptionsMonitor<KeycloakOptions>` where appropriate.
  - Files: `Keycloak/Extensions/ServiceCollectionExtensions.cs`, consumers of `KeycloakOptions`

- [ ] **Deprecate manual `HttpClient` construction path**
  - README still shows `new KeycloakClient(new KeycloakOptions { ... })`, which calls `CreateDefaultHttpClient()` internally.
  - Mark legacy constructors `[Obsolete]` with guidance to use DI, or require an explicit `HttpClient` parameter.
  - Files: `Keycloak/Clients/KeycloakClient.cs`, `README.md`

---

## Code quality and packaging

- [ ] **Enable nullable reference types**
  - Project uses C# 8 on netstandard2.0 but nullable is not enabled.
  - Add `<Nullable>enable</Nullable>` to catch null issues in DTOs and service returns.
  - File: `Keycloak/Keycloak.csproj`

- [ ] **Modernize target frameworks**
  - Consider multi-targeting `netstandard2.0` + `net8.0` (or later) for newer DI/HTTP APIs while keeping broad compatibility.
  - File: `Keycloak/Keycloak.csproj`

- [ ] **Align package versions**
  - `Microsoft.Extensions.*` is on 5.0.0 while IdentityModel is on 7.1.2.
  - Bump Extensions to 8.x (or align with the test project's `net8.0`) to reduce compatibility surprises.
  - File: `Keycloak/Keycloak.csproj`

- [ ] **Add NuGet metadata**
  - README mentions a NuGet package, but the `.csproj` has no `PackageId`, version, description, or README packing.
  - Add metadata if publishing is planned.
  - File: `Keycloak/Keycloak.csproj`

- [ ] **Add logging**
  - No `ILogger` usage anywhere.
  - Log token acquisition failures, API errors (without secrets), and middleware rejection reasons (at debug level).
  - Files: `Keycloak/Clients/KeycloakClient.cs`, `Keycloak/Helpers/KeycloakHttpHelper.cs`, `Keycloak/Middleware/TokenMiddleware.cs`

---

## Testing

- [ ] **`KeycloakClient` token acquisition/refresh tests**
  - Use `HttpMessageHandler` fakes to test token fetch, cache reuse, and refresh on expiry.
  - File: `Keycloak.Test/` (new test class)

- [ ] **Full middleware pipeline tests**
  - Test 401 vs 200 paths through the actual middleware pipeline, not just `BuildValidationParameters`.
  - File: `Keycloak.Test/TokenMiddlewareTest.cs`

- [ ] **`AddKeycloak` DI registration smoke test**
  - Verify services resolve correctly from a `ServiceCollection`.
  - File: `Keycloak.Test/` (new test class)

- [ ] **Optional: integration tests with real Keycloak**
  - Consider Testcontainers + real Keycloak for contract testing.
  - File: `Keycloak.Test/`

---

## Smaller cleanups

- [ ] **Remove duplicate middleware extension**
  - `UseTokenMiddleware` and `UseKeycloakTokenValidation` are identical.
  - Keep one public name; mark the other `[Obsolete]`.
  - File: `Keycloak/Middleware/TokenMiddleware.cs`

- [ ] **Clean up `EventQuery` JSON attributes**
  - `[JsonProperty]` attributes are misleading if the class is only used for query-string building.
  - Use plain properties or a dedicated query builder.
  - File: `Keycloak/Entities/Realm/EventQuery.cs`

- [ ] **Consolidate legacy `TokenValidator` helpers**
  - `CheckIssuer(Payload, ...)` and similar methods overlap with the IdentityModel path.
  - Consolidate or mark as internal if only used by tests.
  - File: `Keycloak/Validators/TokenValidator.cs`

- [ ] **Document `ValidateClientId` / `azp` vs `aud`**
  - `ValidateClientId` checks the `azp` claim, which is correct for Keycloak client tokens.
  - Document when `aud` validation may be needed instead for resource/API tokens.
  - File: `README.md`

---

## Suggested starting order

If picking this up with limited time, start here:

1. Thread-safe token refresh
2. Unified expiry/skew logic in `KeycloakClient`
3. `CancellationToken` support
4. Middleware sets `HttpContext.User`
5. JWKS retry on key rotation

---

## Already done (recent refactors)

- [x] `IHttpClientFactory` integration for DI path
- [x] Static services replaced with DI interfaces (`IUserService`, `IKeyService`, `IRealmService`)
- [x] Typed exceptions instead of returning `null` (`KeycloakApiException`, etc.)
- [x] IdentityModel-based JWT validation in middleware
- [x] Configurable `ServerSkew`, `RealmKeysCacheSeconds`, `ValidateClientId`
- [x] `HttpClient` no longer exposed on `IKeycloakClient`
- [x] Keycloak 17+ URL paths (no `/auth` prefix)
