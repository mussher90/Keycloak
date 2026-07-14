using System;

namespace Keycloak.Constants.Enums
{
    public enum RequiredActionsEnum
    {
        UpdatePassword,
        VerifyEmail,
        ConfigureOTP,
        UpdateProfile,
        TermsAndConditions,
    }

    public static class RequiredActionsEnumExtensions
    {
        public static string ToKeycloakValue(this RequiredActionsEnum action)
        {
            switch (action)
            {
                case RequiredActionsEnum.UpdatePassword:
                    return "UPDATE_PASSWORD";
                case RequiredActionsEnum.VerifyEmail:
                    return "VERIFY_EMAIL";
                case RequiredActionsEnum.UpdateProfile:
                    return "UPDATE_PROFILE";
                case RequiredActionsEnum.ConfigureOTP:
                    return "CONFIGURE_TOTP";
                case RequiredActionsEnum.TermsAndConditions:
                    return "TERMS_AND_CONDITIONS";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown required action.");
            }
        }
    }
}
