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
                Name = "Cardioology"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Dental Care"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Surgery"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Neurology"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Orthopaedy"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Pediatry"
            });
            context.Specialists.Add(new Specialist()
            {
                Name = "Ophthalmology"
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedFacility(ApplicationContext context)
        {
            context.Facilities.Add(new Facility()
            {
                Name = "BACHH MAI HOSPITAL",
                Address = "78 Giai Phong Street, Phuong Mai, Dong Da, Hanoi"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "VIET DUC HOSPITAL",
                Address = "40 Trang Thi, Hoan Kiem, Hanoi"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "CENTRAL CHILDREN'S HOSPITAL",
                Address = "18/879 La Thanh, Dong Da, Hanoi"
            });
            context.Facilities.Add(new Facility()
            {
                Name = "CENTRAL MEDICINE HOSPITAL",
                Address = "Thon Bau, Kim Chung commune, Dong Anh district, Hanoi city."
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedDoctor(ApplicationContext context)
        {
            var doctor = new Doctor()
            {
                UserId = 1,
                FacilityId = 1,
                SpecialistId = 1
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
