namespace TasksApi.Models.DTO
{
    public class AddAppUserDTO
    {
        public string Email { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
