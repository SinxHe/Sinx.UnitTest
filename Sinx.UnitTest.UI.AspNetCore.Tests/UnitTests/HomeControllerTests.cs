using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Sinx.UnitTest.Domain.Model;
using Sinx.UnitTest.UI.AspNetCore.Controllers;
using Sinx.UnitTest.UI.AspNetCore.Models;
using Sinx.UnitTest.UI.AspNetCore.ViewModels;

namespace Sinx.UnitTest.UI.AspNetCore.Tests
{
	/// <summary>
	/// 测试Controller
	/// </summary>
	/// <remarks>
	/// 测试IActionResult的类型
	/// 测试IActionResult.ViewData.Model的类型和值
	/// </remarks>
    public class HomeControllerTests
    {
		[Fact]
	    public async Task Index_ReturnsAViewResult_WithAListOfBrainstormSessions()
	    {
			// Arrange
		    var mockRepo = new Mock<IBrainstormSessionRepository>();
			// 为被模拟的类型的带有返回值的方法设置一个返回值
		    mockRepo.Setup(repo => repo.ListAsync()).Returns(Task.FromResult(GetTestSessions()));
		    var controller = new HomeController(mockRepo.Object);
			
			// Act
		    var result = await controller.Index();

			// Assert
		    var viewResult = Assert.IsType<ViewResult>(result);
		    var model = Assert.IsAssignableFrom<IEnumerable<StormSessionViewModel>>(
			    viewResult.ViewData.Model);
		    Assert.Equal(2, model.Count());
		}

	    [Fact]
	    public async Task IndexPost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
	    {
			// Arrange
		    var mockRepo = new Mock<IBrainstormSessionRepository>();
		    mockRepo.Setup(repo => repo.ListAsync()).Returns(Task.FromResult(GetTestSessions()));
		    var controller = new HomeController(mockRepo.Object);
		    controller.ModelState.AddModelError("SessionName", "Required");
		    var newSession = new NewSessionModel();

			// Act
		    var result = await controller.Index(newSession);

			// Assert
		    var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
			// SerializableError 一个存储了ModelState信息的可序列化容器
		    Assert.IsType<SerializableError>(badRequestObjectResult.Value);
	    }

		[Fact]
	    public async Task IndexPost_ReturnsARedirectAndAddsSession_WhenModelStateIsValid()
	    {
			// Arrange
		    var mockRepo = new Mock<IBrainstormSessionRepository>();
			// "It" 用来设置被Mock类型的某个方法的参数需要符合的条件
			// 把上面设置的期望标记为可验证的, 意味着在调用Mock.Verify()的时候上面的条件成立了
		    mockRepo.Setup(repo => repo.AddAsync(It.IsAny<BrainstormSession>())).Returns(Task.CompletedTask).Verifiable();
		    var controller = new HomeController(mockRepo.Object);
		    var newSession = new NewSessionModel
		    {
				SessionName = "Test Name"
		    };

			// Act
		    var result = await controller.Index(newSession);

			// Assert
		    var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		    Assert.Null(redirectToActionResult.ControllerName);
		    Assert.Equal("Index", redirectToActionResult.ActionName);
			// 确认所有的期望都被满足了
		    mockRepo.Verify();
	    }

	    private static List<BrainstormSession> GetTestSessions()
	    {
		    var sessions = new List<BrainstormSession>
		    {
			    new BrainstormSession
			    {
				    DateCreated = new DateTime(2016, 7, 2),
				    Id = 1,
				    Name = "Test One"
			    },
			    new BrainstormSession
			    {
				    DateCreated = new DateTime(2016, 7, 1),
				    Id = 2,
				    Name = "Test Two"
			    }
		    };
		    return sessions;
	    }
	}
}
