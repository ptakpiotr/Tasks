using Microsoft.EntityFrameworkCore;
using TasksApi.Models;

namespace TasksApi.Data
{
    public class DataDbContext(DbContextOptions<DataDbContext> opts) : DbContext(opts)
    {
        public DbSet<SingleTask> Tasks => Set<SingleTask>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SingleTask>(p => p.HasIndex([nameof(SingleTask.UserId)], name: $"ix_{nameof(SingleTask)}_{nameof(SingleTask.UserId)}"));
        }
    }
}
