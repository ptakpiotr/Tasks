namespace TasksApi.Models.DTO
{
    public class ResetPasswordStartDTO
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = default!;
    }
}
