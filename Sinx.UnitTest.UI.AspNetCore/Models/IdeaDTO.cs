using System;

namespace Sinx.UnitTest.UI.AspNetCore.Models
{
    public class IdeaDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}