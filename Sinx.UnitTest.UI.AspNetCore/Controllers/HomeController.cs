using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sinx.UnitTest.UI.AspNetCore.Models;
using Sinx.UnitTest.UI.AspNetCore.ViewModels;
using Sinx.UnitTest.Domain.Model;

namespace Sinx.UnitTest.UI.AspNetCore.Controllers
{
	public class HomeController : Controller
	{
		private readonly IBrainstormSessionRepository _sessionRepository;

		public HomeController(IBrainstormSessionRepository sessionRepository)
		{
			_sessionRepository = sessionRepository;
		}

		public async Task<IActionResult> Index()
		{
			var sessionList = await _sessionRepository.ListAsync();

			var model = sessionList.Select(session => new StormSessionViewModel()
			{
				Id = session.Id,
				DateCreated = session.DateCreated,
				Name = session.Name,
				IdeaCount = session.Ideas.Count
			});

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Index(NewSessionModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			await _sessionRepository.AddAsync(new BrainstormSession()
			{
				DateCreated = DateTimeOffset.Now,
				Name = model.SessionName
			});

			return RedirectToAction("Index");
		}
	}
}
