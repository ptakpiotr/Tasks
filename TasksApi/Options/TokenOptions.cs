using System.ComponentModel.DataAnnotations;

namespace TasksApi.Options
{
    public class TokenOptions
    {
        [Required]
        public string TokenPrefix { get; set; } = default!;
    }
}
