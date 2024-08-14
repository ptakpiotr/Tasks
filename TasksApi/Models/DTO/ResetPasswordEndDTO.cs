namespace TasksApi.Models.DTO
{
    public class ResetPasswordEndDTO
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = default!;

        public string NewPassword { get; set; } = default!;
    }
}
