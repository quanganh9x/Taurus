using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Models;

namespace Taurus.Data
{
    public class DbInitialize
    {
        public static void DbDefaultValue(ModelBuilder builder)
        {
            builder.Entity<Doctor>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Customer>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Room>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Session>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Question>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Answer>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Facility>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Specialist>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<Notification>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<CustomerVote>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<CustomerFlag>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<DoctorVote>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
            builder.Entity<DoctorFlag>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("getdate()");
        }

        public static void DbDisableCascadeForSomeModels(ModelBuilder builder)
        {
            builder.Entity<Question>()
            .HasMany(p => p.Answers)
            .WithOne(p=> p.Question)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Room>()
            .HasMany(p => p.Sessions)
            .WithOne(p => p.Room)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
            .HasMany(p => p.CustomerFlags)
            .WithOne(p => p.Origin)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
            .HasMany(p => p.CustomerVotes)
            .WithOne(p => p.Origin)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
            .HasMany(p => p.DoctorVotes)
            .WithOne(p => p.Origin)
            .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
            .HasMany(p => p.DoctorFlags)
            .WithOne(p => p.Origin)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
