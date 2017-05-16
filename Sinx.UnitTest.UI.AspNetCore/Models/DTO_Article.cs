using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Sinx.UnitTest.UI.AspNetCore.Models
{
    public class DTO_Article
    {
	    public int Id { get; set; }
	    public string Title { get; set; }
	    public string Content { get; set; }
	    public DateTime CreateTime { get; set; }
	}
}
