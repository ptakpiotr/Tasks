using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using TasksApi.Models.DTO;

namespace TasksApi.Services
{
    public class AppIdentityService(IdentityDbContext dbContext,
        IDataProtectionProvider protectionProvider,
        IPasswordHasher<AppUser> passwordHasher,
        IOptions<Options.TokenOptions> tokenOpts,
        IMapper mapper) : IAppIdentityService
    {
        public async Task<OneOf<AppUserResult, ErrorResult>> AddUserAsync(AddAppUserDTO appUser)
        {
            AppUser user = mapper.Map<AppUser>(appUser);

            AppUser? existingUser = await GetUserByEmailAsync(user.Email).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is not null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoSuccess);
            }

            string passwordHash = passwordHasher.HashPassword(user, appUser.Password);

            user.PasswordHash = passwordHash;

            await dbContext.Users.AddAsync(user).ConfigureAwait(false);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);
            return new AppUserResult(user.Id);
        }

        public async Task<OneOf<AppUser, ErrorResult>> CheckAndReturnUserPasswordAsync(LoginAppUserDTO appUser)
        {
            AppUser? existingUser = await GetUserByEmailAsync(appUser.Email).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            PasswordVerificationResult verificationResult = passwordHasher
                .VerifyHashedPassword(existingUser, existingUser.PasswordHash, appUser.Password);

            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);
            return verificationResult switch
            {
                PasswordVerificationResult.Success => existingUser,
                _ => new ErrorResult(AppConstants.ErrorMessages.NoSuccess)
            };
        }

        public async Task<OneOf<AppUserResult, ErrorResult>> ChangePasswordAsync(ChangePasswordDTO changePassword)
        {
            AppUser? existingUser = await GetUserByIdAsync(changePassword.Id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            PasswordVerificationResult verificationResult = passwordHasher
                .VerifyHashedPassword(existingUser, existingUser.PasswordHash, changePassword.OldPassword);

            if (verificationResult is PasswordVerificationResult.Success)
            {
                string passwordHash = passwordHasher.HashPassword(existingUser, changePassword.NewPassword);
                existingUser.PasswordHash = passwordHash;

                await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);
                return new AppUserResult(existingUser.Id);
            }

            return new ErrorResult(AppConstants.ErrorMessages.NoSuccess);
        }

        public async Task<OneOf<ResetPasswordStartResult, ErrorResult>> ResetPasswordStartAsync(Guid id)
        {
            AppUser? existingUser = await GetUserByIdAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            IDataProtector protector = protectionProvider.CreateProtector($"{tokenOpts.Value.TokenPrefix}_{existingUser.Id}");

            //NOT THE MOST SECURE - FOR DEMONSTRATION PURPOSES ONLY
            return new ResetPasswordStartResult(new()
            {
                Id = existingUser.Id,
                Token = protector.Protect(DateTime.Today.ToString())
            });

        }

        public async Task<OneOf<AppUserResult, ErrorResult>> ResetPasswordEndAsync(ResetPasswordEndDTO resetPasswordEnd)
        {
            AppUser? existingUser = await GetUserByIdAsync(resetPasswordEnd.Id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            IDataProtector protector = protectionProvider.CreateProtector($"{tokenOpts.Value.TokenPrefix}_{existingUser.Id}");

            string unprotected = protector.Unprotect(resetPasswordEnd.Token);

            if (unprotected == DateTime.Today.ToString())
            {
                string passwordHash = passwordHasher.HashPassword(existingUser, resetPasswordEnd.NewPassword);
                existingUser.PasswordHash = passwordHash;

                await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);
                return new AppUserResult(existingUser.Id);
            }

            return new ErrorResult(AppConstants.ErrorMessages.InvalidToken);
        }

        public async Task<OneOf<AppUserResult, ErrorResult>> DeleteUserAsync(Guid id)
        {
            AppUser? existingUser = await GetUserByIdAsync(id).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            dbContext.Users.Remove(existingUser);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new AppUserResult(id);
        }

        public async Task<OneOf<AppUserResult, ErrorResult>> AddClaimAsync(AddClaimDTO addClaim)
        {
            AppUser? existingUser = await GetUserByIdAsync(addClaim.UserId).ConfigureAwait(ConfigureAwaitOptions.None);

            if (existingUser is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            AppClaim claim = mapper.Map<AppClaim>(addClaim);

            await dbContext.Claims.AddAsync(claim).ConfigureAwait(false);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new AppUserResult(existingUser.Id);
        }

        public async Task<OneOf<Success, ErrorResult>> RemoveClaimAsync(RemoveClaimDTO removeClaim)
        {
            AppClaim? claim = await dbContext.Claims.FirstOrDefaultAsync(c => c.Id == removeClaim.Id).ConfigureAwait(false);

            if (claim is null)
            {
                return new ErrorResult(AppConstants.ErrorMessages.NoData);
            }

            dbContext.Claims.Remove(claim);
            await CommitChangesAsync().ConfigureAwait(ConfigureAwaitOptions.None);

            return new Success();
        }

        public Task<AppUser?> GetUserByEmailAsync(string email) => dbContext.Users.Include(c => c.Claims).FirstOrDefaultAsync(u => u.Email == email);

        public Task<AppUser?> GetUserByUserNameAsync(string userName) => dbContext.Users.Include(c => c.Claims).FirstOrDefaultAsync(u => u.UserName == userName);

        public Task<AppUser?> GetUserByIdAsync(Guid id) => dbContext.Users.Include(c => c.Claims).FirstOrDefaultAsync(u => u.Id == id);

        public async Task CommitChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
