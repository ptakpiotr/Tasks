namespace TasksApi.Models.DTO
{
    public class AddClaimDTO
    {
        public string Type { get; set; } = default!;

        public string Value { get; set; } = default!;

        public Guid UserId { get; set; }
    }
}
