namespace TasksApi.Models
{
    public record ErrorResult(string Message, Exception? e = default);
}
