using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OneOf;
using TasksApi.Models.DTO;

namespace TasksApi.Services
{
    public class TasksService(DataDbContext dbContext, IMapper mapper) : ITasksService
    {
        public async Task<SingleTaskResult> AddSingleTaskAsync(AddTaskDTO addTask, Guid userId)
        {
            SingleTask task = mapper.Map<SingleTask>(addTask);
            task.UserId = userId;

            await dbContext.Tasks.AddAsync(task).ConfigureAwait(false);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new(task.Id);
        }


        public async Task<OneOf<SingleTaskResult, ErrorResult>> EditSingleTaskAsync(EditTaskDTO editTask, Guid id, Guid userId)
        {
            SingleTask? task = await GetSingleTaskByIdAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (task is null || task.UserId != userId)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            if (!string.IsNullOrEmpty(editTask.Content))
            {
                task.Content = editTask.Content;
            }

            if (!string.IsNullOrEmpty(editTask.Name))
            {
                task.Content = editTask.Name;
            }

            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new SingleTaskResult(task.Id);
        }

        public async Task<OneOf<SingleTaskResult, ErrorResult>> DeleteSingleTaskAsync(Guid id, Guid userId)
        {
            SingleTask? task = await GetSingleTaskByIdAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (task is null || task.UserId != userId)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            dbContext.Tasks.Remove(task);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new SingleTaskResult(task.Id);
        }

        public async Task DeleteAllUserTasksAsync(Guid userId)
        {
            List<SingleTask> tasks = await GetTasksByUserAsync(userId).ConfigureAwait(ConfigureAwaitOptions.None);
            dbContext.Tasks.RemoveRange(tasks);

            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);
        }

        public Task<List<SingleTask>> GetTasksByUserAsync(Guid userId) => dbContext.Tasks.AsNoTracking().Where(t => t.UserId == userId).ToListAsync();

        public Task<SingleTask?> GetSingleTaskByIdAsync(Guid id) => dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        public async Task CommitChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
