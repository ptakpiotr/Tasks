using Microsoft.EntityFrameworkCore;
using TasksApi.Data.Interceptors;
using TasksApi.Models;

namespace TasksApi.Data
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> opts, ILogger<IdentityDbContext> logger) : DbContext(opts)
    {
        public DbSet<AppUser> Users => Set<AppUser>();

        public DbSet<AppClaim> Claims => Set<AppClaim>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.AddInterceptors(new IdentitySaveChangesInterceptor(logger));
        }
    }
}
