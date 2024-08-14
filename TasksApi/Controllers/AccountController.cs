using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TasksApi.Filters;
using TasksApi.Helpers;
using TasksApi.Models.DTO;

namespace TasksApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize(AuthenticationSchemes = AppConstants.AuthenticationSchemeName)]
    public class AccountController(IAppIdentityService identityService,
        ILogger<AccountController> logger,
        IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AddAppUserDTO user)
        {
            var res = await identityService.AddUserAsync(user).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match<ObjectResult>((res) =>
            {
                return Created(nameof(AccountController.Register), new ItemIdDTO<Guid>(res.Obj));
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginAppUserDTO login)
        {
            var res = await identityService.CheckAndReturnUserPasswordAsync(login).ConfigureAwait(ConfigureAwaitOptions.None);


            return await res.Match(async Task<ObjectResult> (res) =>
            {
                ClaimsPrincipal principal = res.ConvertToClaimsPrincipal();
                await HttpContext.SignInAsync(AppConstants.AuthenticationSchemeName, principal);

                return StatusCode(StatusCodes.Status200OK, "");
            }, Task<ObjectResult> (err) =>
            {
                logger.LogError(err.Message);
                return Task.FromResult(StatusCode(StatusCodes.Status400BadRequest, AppConstants.ErrorMessages.NoSuccess));
            });
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePassword)
        {
            var res = await identityService.ChangePasswordAsync(changePassword).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match((res) =>
            {
                return StatusCode(StatusCodes.Status204NoContent, new { });
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpPost("resetpasswordstart/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordStart([FromRoute] Guid id)
        {
            var res = await identityService.ResetPasswordStartAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match<ObjectResult>((res) =>
            {
                return Ok(res.Obj);
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpPost("resetpasswordend")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordEnd([FromBody] ResetPasswordEndDTO resetPasswordEnd)
        {
            var res = await identityService.ResetPasswordEndAsync(resetPasswordEnd).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match<ObjectResult>((res) =>
            {
                return Ok(new ItemIdDTO<Guid>(res.Obj));
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpPost("addclaim")]
        public async Task<IActionResult> AddClaim([FromBody] AddClaimDTO addClaim)
        {
            var res = await identityService.AddClaimAsync(addClaim).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match<ObjectResult>((res) =>
            {
                return Created(nameof(AccountController.AddClaim), new ItemIdDTO<Guid>(res.Obj));
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpPost("removeclaim")]
        public async Task<IActionResult> RemoveClaim([FromBody] RemoveClaimDTO removeClaim)
        {
            var res = await identityService.RemoveClaimAsync(removeClaim).ConfigureAwait(ConfigureAwaitOptions.None);


            return res.Match<ObjectResult>((res) =>
            {
                return StatusCode(StatusCodes.Status204NoContent, new { });
            }, (err) =>
            {
                logger.LogError(err.Message);
                return BadRequest(AppConstants.ErrorMessages.NoSuccess);
            });
        }

        [HttpDelete("{userId}")]
        [ServiceFilter<ValidateUserActionFilter>]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var res = await identityService.DeleteUserAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);


            return await res.Match<Task<ObjectResult>>(async (res) =>
            {
                await publishEndpoint.Publish(new DeleteUserInfo(res.Obj));
                return StatusCode(StatusCodes.Status204NoContent, new { });
            }, (err) =>
            {
                logger.LogError(err.Message);
                return Task.FromResult(StatusCode(StatusCodes.Status400BadRequest, AppConstants.ErrorMessages.NoSuccess));
            });
        }
    }
}
