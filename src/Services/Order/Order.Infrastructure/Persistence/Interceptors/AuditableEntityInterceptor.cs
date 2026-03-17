using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Order.Domain.Abstractions;

namespace Order.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetEntityAuditTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetEntityAuditTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetEntityAuditTimestamps(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModified = DateTime.UtcNow;
            }
        }
    }
}

public static class Extensions
{
    /// <summary>
    /// 检测当前实体是否存在新增/修改状态的关联自有实体
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(re =>
        re.TargetEntry != null &&
        re.TargetEntry.Metadata.IsOwned() && // 目标实体是自有实体
        re.TargetEntry.State is EntityState.Added or EntityState.Modified);

}
