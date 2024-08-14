using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ChangeTrackerEntries = System.Collections.Generic.List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry>;

namespace TasksApi.Data.Interceptors
{
    public class IdentitySaveChangesInterceptor(ILogger<IdentityDbContext> logger) : SaveChangesInterceptor
    {
        private const string TAG = "[IDENTITY]";

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                ChangeTrackerEntries changeTrackerChanges = eventData.Context.ChangeTracker.Entries().ToList();

                ChangeTrackerEntries auditableChanges = changeTrackerChanges
                    .Where(c => c.State == EntityState.Modified || c.State == EntityState.Deleted || c.State == EntityState.Added)
                    .ToList();

                foreach (var auditableChange in auditableChanges)
                {
                    logger.LogInformation("{Tag} -> {Operation}", TAG, auditableChange.State);
                }
            }

            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            logger.LogError("{Tag} -> {Content}", TAG, eventData.Exception.Message);

            return base.SaveChangesFailedAsync(eventData, cancellationToken);
        }
    }
}
