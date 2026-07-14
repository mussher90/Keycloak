using Keycloak.Entities.Realm;
using System;
using Xunit;

namespace Keycloak.Test
{
    public class EventQueryTest
    {
        [Fact]
        public void GetQueryString_ReturnsEmpty_WhenNoFiltersSet()
        {
            var query = new EventQuery();

            Assert.Equal(string.Empty, query.GetQueryString());
        }

        [Fact]
        public void GetQueryString_ReturnsSingleParameter_WithLeadingQuestionMark()
        {
            var query = new EventQuery { Client = "my-app" };

            Assert.Equal("?client=my-app", query.GetQueryString());
        }

        [Fact]
        public void GetQueryString_JoinsMultipleParameters_WithAmpersand()
        {
            var query = new EventQuery
            {
                Client = "my-app",
                FromDate = new DateTime(2024, 1, 15),
                Max = 50
            };

            Assert.Equal("?client=my-app&dateFrom=2024-01-15&max=50", query.GetQueryString());
        }

        [Fact]
        public void GetQueryString_RepeatsTypeParameter_ForEachEventType()
        {
            var query = new EventQuery
            {
                Type = new[] { "LOGIN", "LOGOUT" }
            };

            Assert.Equal("?type=LOGIN&type=LOGOUT", query.GetQueryString());
        }

        [Fact]
        public void GetQueryString_EncodesSpecialCharacters()
        {
            var query = new EventQuery { IpAddress = "192.168.1.1" };

            Assert.Equal("?ipAddress=192.168.1.1", query.GetQueryString());
        }
    }
}
