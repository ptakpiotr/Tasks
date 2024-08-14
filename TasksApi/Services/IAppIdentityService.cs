using OneOf;
using OneOf.Types;
using TasksApi.Models;
using TasksApi.Models.DTO;

namespace TasksApi.Services
{
    public interface IAppIdentityService
    {
        Task<OneOf<AppUserResult, ErrorResult>> AddClaimAsync(AddClaimDTO addClaim);
        Task<OneOf<AppUserResult, ErrorResult>> AddUserAsync(AddAppUserDTO appUser);
        Task<OneOf<AppUserResult, ErrorResult>> ChangePasswordAsync(ChangePasswordDTO changePassword);
        Task<OneOf<AppUser, ErrorResult>> CheckAndReturnUserPasswordAsync(LoginAppUserDTO appUser);
        Task CommitChangesAsync();
        Task<OneOf<AppUserResult, ErrorResult>> DeleteUserAsync(Guid id);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<AppUser?> GetUserByIdAsync(Guid id);
        Task<AppUser?> GetUserByUserNameAsync(string userName);
        Task<OneOf<Success, ErrorResult>> RemoveClaimAsync(RemoveClaimDTO removeClaim);
        Task<OneOf<AppUserResult, ErrorResult>> ResetPasswordEndAsync(ResetPasswordEndDTO resetPasswordEnd);
        Task<OneOf<ResetPasswordStartResult, ErrorResult>> ResetPasswordStartAsync(Guid id);
    }
}