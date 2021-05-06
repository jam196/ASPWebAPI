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
            // for entities that inherit from BaseEntity,
            // set UpdatedOn / CreatedOn appropriately
            if (entry.Entity is BaseEntity trackable)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        trackable.CreatedAt = utcNow;

                        entry.Property("CreatedOn").IsModified = false;
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