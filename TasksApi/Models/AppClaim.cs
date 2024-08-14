using System.ComponentModel.DataAnnotations;

namespace TasksApi.Models
{
    public class AppClaim
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; } = default!;

        public string Value { get; set; } = default!;

        public Guid UserId { get; set; }

        [Required]
        public AppUser User { get; set; } = default!;
    }
}
