using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sinx.UnitTest.UI.AspNetCore.Tests.IntegrationTests
{
    public class SessionControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public SessionControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Index_ReturnsCorrectSessionPage()
        {
            // Arrange
            var testSession = Startup.GetTestSession();

            // Arrange & Act
            var response = await _client.GetAsync("/Session/Index/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.True(responseString.Contains(testSession.Name));
        }
    }
}