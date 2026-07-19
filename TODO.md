# Keycloak Library — TODO

Improvement backlog for future work. Items are roughly ordered by impact. Check off items as they are completed.

---

## High impact

- [ ] **Add `CancellationToken` support**
  - No HTTP calls accept cancellation today.
  - Add `CancellationToken cancellationToken = default` to `IKeycloakClient.Send`, service methods, and auth configuration where applicable.
  - Files: `Keycloak/Clients/IKeycloakClient.cs`, `Keycloak/Clients/KeycloakClient.cs`, service interfaces/implementations

---

## Auth UX

- [ ] **Support optional / mixed auth routes**
  - Use standard ASP.NET Core `[AllowAnonymous]` and authorization policies with `AddKeycloakAuthentication()`.
  - Consider documenting common patterns in README.
  - File: `README.md`

- [ ] **Document `ValidateClientId` / `azp` vs `aud`**
  - `ValidateClientId` checks the `azp` claim, which is correct for Keycloak client tokens.
  - Document when `aud` validation may be needed instead for resource/API tokens.
  - File: `README.md`

---

## API design and ergonomics

- [ ] **Add `AddKeycloak` overloads**
  - Currently only `AddKeycloak(IServiceCollection, IConfiguration)`.
  - Consider: `AddKeycloak(services, Action<KeycloakOptions> configure)` and `AddKeycloak(services, KeycloakOptions options)`.
  - Consider `services.AddOptions<KeycloakOptions>().BindConfiguration(...).ValidateOnStart()` instead of manual bind + validate.
  - File: `Keycloak/Extensions/ServiceCollectionExtensions.cs`

- [ ] **Combine admin + auth registration**
  - Consider a single `AddKeycloakAuth()` helper that calls `AddKeycloak()` and `AddKeycloakAuthentication()`.
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
  - Log token acquisition failures and API errors (without secrets).
  - Files: `Keycloak/Clients/KeycloakClient.cs`, `Keycloak/Helpers/KeycloakHttpHelper.cs`

---

## Testing

- [ ] **`KeycloakClient` token acquisition/refresh tests**
  - Partially covered by `KeycloakClientTest` (concurrency, cache reuse, refresh on expiry).
  - File: `Keycloak.Test/KeycloakClientTest.cs`

- [ ] **`AddKeycloak` DI registration smoke test**
  - Verify services resolve correctly from a `ServiceCollection`.
  - File: `Keycloak.Test/` (new test class)

- [ ] **Optional: integration tests with real Keycloak**
  - Consider Testcontainers + real Keycloak for contract testing.
  - File: `Keycloak.Test/`

---

## Smaller cleanups

- [ ] **Clean up `EventQuery` JSON attributes**
  - `[JsonProperty]` attributes are misleading if the class is only used for query-string building.
  - Use plain properties or a dedicated query builder.
  - File: `Keycloak/Entities/Realm/EventQuery.cs`

---

## Suggested starting order

If picking this up with limited time, start here:

1. `CancellationToken` support
2. Configurable HTTP timeouts and resilience
3. `AddKeycloak` overloads
4. `AddKeycloakAuth()` combined registration helper

---

## Already done (recent refactors)

- [x] Removed legacy custom token middleware and duplicate JWT validation helpers
- [x] Unified token expiry checking (`expires_in` + JWT `exp`, shared `ServerSkew`, IdentityModel parsing)
- [x] `AddKeycloakAuthentication()` / `UseKeycloakAuthentication()` JWT bearer integration (net8.0+)
- [x] Thread-safe token refresh in `KeycloakClient` (`SemaphoreSlim` with double-checked locking)
- [x] Multi-target `netstandard2.0` + `net8.0`
- [x] `IHttpClientFactory` integration for DI path
- [x] Static services replaced with DI interfaces (`IUserService`, `IKeyService`, `IRealmService`)
- [x] Typed exceptions instead of returning `null` (`KeycloakApiException`, etc.)
- [x] Configurable `ServerSkew`, `ValidateClientId`
- [x] `HttpClient` no longer exposed on `IKeycloakClient`
- [x] Keycloak 17+ URL paths (no `/auth` prefix)
