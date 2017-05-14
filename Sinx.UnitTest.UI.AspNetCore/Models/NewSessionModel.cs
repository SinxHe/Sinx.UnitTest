using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sinx.UnitTest.UI.AspNetCore.Models
{
	public class NewSessionModel
	{
		[Required]
		public string SessionName { get; set; }
	}
}
