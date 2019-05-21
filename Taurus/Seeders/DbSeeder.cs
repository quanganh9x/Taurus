using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;

namespace Taurus.Seeders
{
    public class DbSeeder
    {
        public static void createSeed(ApplicationContext context)
        {
            
            SeedSpecialist(context);
            Thread.Sleep(1000);
            SeedFacility(context);
            Thread.Sleep(1000);
            SeedStaff(context);
            Thread.Sleep(1000);
            SeedPatient(context);
            Thread.Sleep(1000);
            //SeedCase(context);
            Thread.Sleep(1000);
            SeedAppointment(context);
            Thread.Sleep(1000);
            SeedBill(context);
            Thread.Sleep(1000);

        }   
        public static void SeedBill(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Bills.Add(new Bill()
            {
                Diagnosis = "Đau đầu",
                Note = "Ngày uống 3 lần sau ăn",
                Medicines = "pa nờ na đôn",
            });
            context.SaveChanges();
        }
        public static void SeedAppointment(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Appointments.Add(new Appointment()
            {
                
            });
            context.SaveChanges();
        }
        public static void SeedCase(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Cases.Add(new Case()
            {
                StaffId = "",
                PatientId = "",
                SpecialistId = 1,

            });
            context.SaveChanges();
        }
        public static void SeedSpecialist(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Specialists.Add(new Specialist()
            {
                Name = "Nha Khoa"
            });
            context.SaveChanges();
        }
        public static void SeedFacility(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Facilities.Add(
                new Facility()
                {
                    Name = "Bệnh viện 5 Bs",
                    Address = "Sô 8, Tôn Thất Thuyết, Cầu Giấy, Hà nội."
                });
            context.SaveChanges();
        }

        public static void SeedStaff(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Staffs.Add(new Staff()
            {
                UserId = 1,
                FacilityId = 1,
                SpecialistId = 1,
            });
            context.SaveChanges();
        }
        public static void SeedPatient(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            context.Patients.Add(new Patient()
            {
                UserId = 1,
            });
            context.SaveChanges();
           
        }
    }
}
