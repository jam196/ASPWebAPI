using System;
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

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
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