using System.ComponentModel.DataAnnotations;

namespace TasksApi.Models
{
    public class SingleTask
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Content { get; set; } = default!;

        public Guid UserId { get; set; }
    }
}
