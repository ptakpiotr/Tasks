using System.Security.Claims;

namespace TasksApi.Helpers
{
    public static class UserTransformer
    {
        public static ClaimsPrincipal ConvertToClaimsPrincipal(this AppUser appUser)
        {
            List<Claim> claims = [
                new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new(ClaimTypes.Email, appUser.Email)
            ];

            foreach (var appClaim in appUser.Claims)
            {
                claims.Add(new(appClaim.Type, appClaim.Value));
            }

            ClaimsIdentity identity = new(claims, AppConstants.AuthenticationSchemeName);

            return new(identity);
        }
    }
}
