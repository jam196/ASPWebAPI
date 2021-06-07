using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public DbSet<WebAPI.Models.User> User { get; set; }
    public DbSet<WebAPI.Models.Bridge> Bridge { get; set; }
    public DbSet<WebAPI.Models.History> History { get; set; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        OnBeforeSaving();
        return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
            cancellationToken));
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity trackable)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        trackable.UpdatedAt = utcNow;

                        entry.Property("UpdatedAt").IsModified = false;
                        break;

                    case EntityState.Added:
                        trackable.CreatedAt = utcNow;
                        trackable.UpdatedAt = utcNow;
                        break;
                }
            }
        }
    }
}