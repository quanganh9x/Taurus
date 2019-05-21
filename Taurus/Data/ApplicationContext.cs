using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taurus.Areas.Identity.Models;
using Taurus.Models;

namespace Taurus.Data
{
    public class ApplicationContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Staff>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Staff>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Patient>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Patient>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Case>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Case>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Bill>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Bill>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("getdate()");
        }
    }
}
