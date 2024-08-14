namespace TasksApi.Models.DTO
{
    public class AddTaskDTO
    {
        public string Name { get; set; } = default!;

        public string Content { get; set; } = default!;
    }
}
