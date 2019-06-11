using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;
using Taurus.Models.Enums;

namespace Taurus.Data
{
    public class DbSeeder
    {
        public static async Task Seed(ApplicationContext context)
        {
            await context.Database.EnsureCreatedAsync();
            if (context.Doctors.Any())
            {
                return;   // DB has been seeded
            }
            await SeedSpecialist(context);
            await SeedFacility(context);
            await SeedDoctor(context);
            await SeedCustomer(context);
        }

        public static async Task SeedSpecialist(ApplicationContext context)
        {
            context.Specialists.Add(new Specialist()
            {
                Name = "Phụ Khoa"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Sản Khoa"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Nội Khoa"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Ngoại Khoa"
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedFacility(ApplicationContext context)
        {
            context.Facilities.Add(new Facility()
            {
                Name = "Siêu Bệnh viện",
                Address = "8 Tôn Thất Thuyết, Cầu Giấy, Hà Nội"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "Bệnh viện Neymar",
                Address = "10 Neymar, Paris, France"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "Bệnh viện Messi",
                Address = "10 Messi, Messi, Barcelona"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "Bệnh viện Ronaldo",
                Address = "7 Juv, Torino, Italy"
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedDoctor(ApplicationContext context)
        {
            var doctor = new Doctor()
            {
                UserId = 1,
                FacilityId = 1,
                SpecialistId = 1,
                IsDoctorOfTheMonth = true
            };
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            doctor = new Doctor()
            {
                UserId = 2,
                FacilityId = 2,
                SpecialistId = 2
            };
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();
        }

        public static async Task SeedCustomer(ApplicationContext context)
        {
            var customer = new Customer()
            {
                UserId = 3
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            customer = new Customer()
            {
                UserId = 4
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            customer = new Customer()
            {
                UserId = 5
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
        }
    }
}
