using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Taurus.Areas.Identity.Models;
using Taurus.Models;

namespace Taurus.Data
{
    public class ApplicationContext : IdentityDbContext<User, Role, int>
    {
        private readonly IConfiguration _configuration;


        public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<Staff> Staffs { get; set; }

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
        }
    }
}
