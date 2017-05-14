using System.ComponentModel.DataAnnotations;

namespace Sinx.UnitTest.UI.AspNetCore.Models
{
    public class NewIdeaModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000000)]
        public int SessionId { get; set; }
    }
}