using MassTransit;
using TasksApi.Models;
using TasksApi.Services;

namespace TasksApi.Consumers
{
    public class DeleteUserConsumer(ITasksService tasksService) : IConsumer<DeleteUserInfo>
    {
        public async Task Consume(ConsumeContext<DeleteUserInfo> context)
        {
            await tasksService.DeleteAllUserTasksAsync(context.Message.UserId).ConfigureAwait(ConfigureAwaitOptions.None);
        }
    }
}
