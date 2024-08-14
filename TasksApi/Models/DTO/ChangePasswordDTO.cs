namespace TasksApi.Models.DTO
{
    public class ChangePasswordDTO
    {
        public Guid Id { get; set; }

        public string OldPassword { get; set; } = default!;

        public string NewPassword { get; set; } = default!;
    }
}
