using OneOf;
using TasksApi.Models;
using TasksApi.Models.DTO;

namespace TasksApi.Services
{
    public interface ITasksService
    {
        Task<SingleTaskResult> AddSingleTaskAsync(AddTaskDTO addTask, Guid userId);
        Task CommitChangesAsync();
        Task DeleteAllUserTasksAsync(Guid userId);
        Task<OneOf<SingleTaskResult, ErrorResult>> DeleteSingleTaskAsync(Guid id, Guid userId);
        Task<OneOf<SingleTaskResult, ErrorResult>> EditSingleTaskAsync(EditTaskDTO editTask, Guid id, Guid userId);
        Task<SingleTask?> GetSingleTaskByIdAsync(Guid id);
        Task<List<SingleTask>> GetTasksByUserAsync(Guid userId);
    }
}