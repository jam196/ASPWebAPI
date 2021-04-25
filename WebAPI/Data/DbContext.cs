using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext (DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public DbSet<WebAPI.Models.User> User { get; set; }
}