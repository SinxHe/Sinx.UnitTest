using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sinx.UnitTest.Domain.Model;
using Sinx.UnitTest.UI.AspNetCore.Controllers;
using Sinx.UnitTest.UI.AspNetCore.ViewModels;
using Xunit;

namespace Sinx.UnitTest.UI.AspNetCore.Tests.UnitTests
{
    public class SessionControllerTests
    {
        [Fact]
        public async Task Index_ReturnsARedirectToIndexHome_WhenIdIsNull()
        {
            // Arrange
            var controller = new SessionController(sessionRepository: null);

            // Act
            var result = await controller.Index(id: null);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Index_ReturnsContentWithSessionNotFound_WhenSessionNotFound()
        {
            // Arrange
            const int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .Returns(Task.FromResult((BrainstormSession)null));
            var controller = new SessionController(mockRepo.Object);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("Session not found.", contentResult.Content);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithStormSessionViewModel()
        {
            // Arrange
            const int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .Returns(Task.FromResult(GetTestSessions().FirstOrDefault(s => s.Id == testSessionId)));
            var controller = new SessionController(mockRepo.Object);

            // Act
            var result = await controller.Index(testSessionId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<StormSessionViewModel>(viewResult.ViewData.Model);
            Assert.Equal("Test One", model.Name);
            Assert.Equal(2, model.DateCreated.Day);
            Assert.Equal(testSessionId, model.Id);
        }

        private static IEnumerable<BrainstormSession> GetTestSessions()
        {
	        yield return new BrainstormSession()
	        {
		        DateCreated = new DateTime(2016, 7, 2),
		        Id = 1,
		        Name = "Test One"
	        };
	        yield return new BrainstormSession()
	        {
		        DateCreated = new DateTime(2016, 7, 1),
		        Id = 2,
		        Name = "Test Two"
	        };
        }
    }
}
