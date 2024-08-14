using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;
using TasksApi.Models.DTO;

namespace TasksApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = AppConstants.AuthenticationSchemeName)]
    public class TasksController(ITasksService tasksService, ILogger<TasksController> logger, IOutputCacheStore store) : ControllerBase
    {
        [HttpGet]
        [OutputCache(Tags = ["Tasks"], VaryByQueryKeys = ["userId"])]
        public async Task<IActionResult> GetTasksByUserId([FromQuery] Guid userId)
        {
            List<SingleTask> tasks = await tasksService.GetTasksByUserAsync(userId).ConfigureAwait(ConfigureAwaitOptions.None);

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AddSingleTask([FromBody] AddTaskDTO addTask, CancellationToken token)
        {
            Claim? idClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return BadRequest();
            }

            Guid.TryParse(idClaim.Value?.ToString(), out Guid userId);

            SingleTaskResult res = await tasksService.AddSingleTaskAsync(addTask, userId).ConfigureAwait(ConfigureAwaitOptions.None);
            await store.EvictByTagAsync("Tasks", token).ConfigureAwait(false);

            return Created(nameof(TasksController.AddSingleTask), res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSingleTask([FromRoute] Guid id, [FromBody] EditTaskDTO editTask)
        {
            Claim? idClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return BadRequest();
            }

            Guid.TryParse(idClaim.Value?.ToString(), out Guid userId);

            var res = await tasksService.EditSingleTaskAsync(editTask, id, userId).ConfigureAwait(ConfigureAwaitOptions.None);

            return res.Match((res) =>
            {
                return StatusCode(StatusCodes.Status204NoContent, new { });
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpDelete("{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteSingleTask([FromRoute] Guid id)
        {
            Claim? idClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                return BadRequest();
            }

            Guid.TryParse(idClaim.Value?.ToString(), out Guid userId);

            var res = await tasksService.DeleteSingleTaskAsync(id, userId).ConfigureAwait(ConfigureAwaitOptions.None);

            return res.Match((res) =>
            {
                return StatusCode(StatusCodes.Status204NoContent, new { });
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }
    }
}
