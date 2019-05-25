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

        public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Room> Bills { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DoctorVote> DoctorVotes { get; set; }
        public DbSet<DoctorFlag> DoctorFlags { get; set; }
        public DbSet<CustomerVote> CustomerVotes { get; set; }
        public DbSet<CustomerFlag> CustomerFlags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            DbInitialize.DbDefaultValue(builder);
            DbInitialize.DbDisableCascadeForSomeModels(builder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(_configuration["ConnectionStrings:DefaultConnection"]);
        }
    }
}
