# Keycloak

A C# library for interacting with a [Keycloak](https://www.keycloak.org/) authentication server (Keycloak 17+). It provides admin API clients, JWT validation, and ASP.NET Core middleware.

## Requirements

- .NET Standard 2.0+ (library)
- Keycloak 17+ (modern URL paths without `/auth` prefix)

## Installation

Reference the `Keycloak` project or NuGet package in your application.

## Configuration

### Recommended: nested `Keycloak` section

```json
{
  "Keycloak": {
    "ServerUrl": "https://keycloak.example.com",
    "Realm": "myrealm",
    "ClientId": "my-service",
    "ClientSecret": "your-client-secret",
    "ServerSkew": 30,
    "RealmKeysCacheSeconds": 3600,
    "ValidateClientId": true
  }
}
```

### Legacy flat keys (still supported)

```json
{
  "KeycloakServer": "https://keycloak.example.com",
  "Realm": "myrealm",
  "ClientId": "my-service",
  "ClientSecret": "your-client-secret"
}
```

| Setting | Description |
|---------|-------------|
| `ServerUrl` | Base Keycloak URL (no trailing `/auth`) |
| `Realm` | Realm name |
| `ClientId` / `ClientSecret` | Service account for admin API (client credentials) |
| `ServerSkew` | Clock skew in seconds for token expiry validation |
| `RealmKeysCacheSeconds` | How long middleware caches JWKS keys (default 3600) |
| `ValidateClientId` | Whether middleware validates the token `azp` claim |

## Dependency injection

```csharp
using Keycloak.Extensions;
using Keycloak.Middleware;

// Program.cs / Startup.cs
builder.Services.AddMemoryCache();
builder.Services.AddKeycloak(builder.Configuration);

var app = builder.Build();
app.UseKeycloakTokenValidation();
```

`AddKeycloak` registers:

- `KeycloakOptions`
- `IKeycloakClient`
- `IUserService`
- `IKeyService`
- `IRealmService`

## Usage examples

### Create a user

```csharp
public class MyController
{
    private readonly IUserService _users;

    public MyController(IUserService users) => _users = users;

    public async Task<string> Register(string username, string email)
    {
        return await _users.CreateUserAsync(new UserRepresentation
        {
            UserName = username,
            EmailAddress = email,
            Enabled = true,
        });
    }
}
```

### Reset a password

```csharp
await _users.ResetUserPasswordAsync(new CredentialRepresentation
{
    Value = "new-password",
    Temporary = true,
}, userId);
```

### Send a required-action email

```csharp
using Keycloak.Constants.Enums;

await _users.SendEmailAsync(userId, new[]
{
    RequiredActionsEnum.UpdatePassword,
    RequiredActionsEnum.VerifyEmail,
});
```

### Query realm events

```csharp
var events = await _realmService.GetRealmEventsAsync(new EventQuery
{
    Client = "my-app",
    FromDate = DateTime.UtcNow.AddDays(-7),
    Max = 100,
});
```

### Manual client construction (without DI)

```csharp
var client = new KeycloakClient(new KeycloakOptions
{
    ServerUrl = "https://keycloak.example.com",
    Realm = "myrealm",
    ClientId = "my-service",
    ClientSecret = "secret",
});

var userService = new UserService(client);
```

## Error handling

API failures throw `KeycloakApiException` with `StatusCode`, `ResponseBody`, and `RequestUri`. Token acquisition failures throw `KeycloakAuthenticationException`. Configuration problems throw `KeycloakConfigurationException`.

```csharp
try
{
    await _users.CreateUserAsync(user);
}
catch (KeycloakApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
{
    // User already exists
}
```

## Breaking changes (recent versions)

If upgrading from an older version of this library:

1. **URL paths** — Keycloak 17+ uses `/realms/...` and `/admin/realms/...` instead of `/auth/realms/...`.
2. **Static services removed** — Use `IUserService`, `IKeyService`, `IRealmService` via DI instead of `UserService.CreateUserAsync(client, ...)`.
3. **Null on failure removed** — Service methods throw `KeycloakApiException` instead of returning `null`.
4. **`HttpResponseMessage` return types removed** — Update/Delete/Email methods return `Task` and throw on failure.
5. **`IKeycloakClient.Client` removed** — `HttpClient` is no longer exposed on the interface.
6. **Namespaces updated** — `TokenValidator` is in `Keycloak.Validators`, `TokenValidationParameters` in `Keycloak.Entities`, `ValidationStatusCode` in `Keycloak.Constants.Enums`.

## Middleware

The token middleware validates `Authorization: Bearer` tokens on incoming requests. It caches realm signing keys, validates issuer (`{ServerUrl}/realms/{Realm}`), and optionally validates the client ID (`azp`).

Requires `IMemoryCache` and `AddKeycloak()` to be registered before `UseKeycloakTokenValidation()`.

## Testing

```bash
dotnet test Keycloak.Test/Keycloak.Test.csproj
```

## License

See repository license file.
