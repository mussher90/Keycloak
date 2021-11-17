namespace Keycloak.Enums
{
    public enum ValidationStatusCode
    {
        Ok,
        IncorrectFormat,
        IncorrectIssuer,
        IncorrectClient,
        InvalidWebOrigins,
        Expired,
        MissingAlgorithm,
        MissingSigningKey,
        InvalidSignature
    }
}
