using System.ComponentModel.DataAnnotations;

namespace TasksApi.Models
{
    public class AppUser
    {
        [Key]
        public Guid Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = default!;

        [Length(3, 50)]
        public string UserName { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public IList<AppClaim> Claims { get; set; } = new List<AppClaim>();
    }
}
