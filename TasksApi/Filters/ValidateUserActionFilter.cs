using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace TasksApi.Filters
{
    public class ValidateUserActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;

            Claim? idClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (idClaim is null)
            {
                context.Result = new UnauthorizedResult();
            }

            if (context.ActionArguments.TryGetValue("id", out var id))
            {
                if (idClaim!.Value != id!.ToString())
                {
                    context.Result = new UnauthorizedResult();

                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
