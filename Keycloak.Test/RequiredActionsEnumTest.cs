using Keycloak.Constants.Enums;
using Keycloak.Entities;
using Keycloak.Entities.Keys;
using System;
using Xunit;

namespace Keycloak.Test
{
    public class RequiredActionsEnumTest
    {
        [Theory]
        [InlineData(RequiredActionsEnum.UpdatePassword, "UPDATE_PASSWORD")]
        [InlineData(RequiredActionsEnum.VerifyEmail, "VERIFY_EMAIL")]
        [InlineData(RequiredActionsEnum.UpdateProfile, "UPDATE_PROFILE")]
        [InlineData(RequiredActionsEnum.ConfigureOTP, "CONFIGURE_TOTP")]
        [InlineData(RequiredActionsEnum.TermsAndConditions, "TERMS_AND_CONDITIONS")]
        public void ToKeycloakValue_ReturnsExpectedAction(RequiredActionsEnum action, string expected)
        {
            Assert.Equal(expected, action.ToKeycloakValue());
        }
    }
}
