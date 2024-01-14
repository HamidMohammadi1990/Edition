using System.Reflection;
using Edition.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Edition.Infrastructure.Persistence.Interceptors;

public class CleanStringPropertyInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return ValueTask.FromResult(result);

        var changedEntities = eventData.Context.ChangeTracker.Entries()
             .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
        foreach (var item in changedEntities)
        {
            if (item.Entity == null)
                continue;
            
            var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var propName = property.Name;
                string? val = (string)property.GetValue(item.Entity, null);

                if (val.HasValue())
                {
                    var newVal = val!.Fa2En().FixPersianChars();
                    if (newVal == val)
                        continue;
                    property.SetValue(item.Entity, newVal, null);
                }
            }
        }
        return ValueTask.FromResult(result);
    }
}
