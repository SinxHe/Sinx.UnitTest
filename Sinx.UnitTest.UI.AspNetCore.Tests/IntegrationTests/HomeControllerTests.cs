using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.UI.AspNetCore.Tests.IntegrationTests
{
    public class HomeControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public HomeControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Returns_InitialListOfBrainstormSessions()
        {
            // Arrange - get a session known to exist
            var testSession = Startup.GetTestSession();

            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(responseString.Contains(testSession.Name));
        }

        [Fact]
        public async Task PostAddsNewBrainstormSession()
        {
            // Arrange
            var testSessionName = Guid.NewGuid().ToString();
	        var data = new Dictionary<string, string> {{"SessionName", testSessionName}};
	        var content = new FormUrlEncodedContent(data);

            // Act
            var response = await _client.PostAsync("/", content);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.ToString());
        }
    }
}