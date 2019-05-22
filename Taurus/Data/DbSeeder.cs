using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;

namespace Taurus.Data
{
    public class DbSeeder
    {
        private static string StaffId = null;
        private static string PatientId = null;
        public static async Task Seed(ApplicationContext context)
        {
            await context.Database.EnsureCreatedAsync();
            if (context.Staffs.Any())
            {
                return;   // DB has been seeded
            }
            await SeedSpecialist(context);
            await SeedFacility(context);
            await SeedStaff(context);
            await SeedPatient(context);
            await SeedCase(context);
            await SeedAppointment(context);
            await SeedBill(context);
        }


        public static async Task SeedBill(ApplicationContext context)
        {
            context.Bills.Add(new Bill()
            {
                CaseId = 1,
                Diagnosis = "Xuất tinh sớm. 5 phút ggwp",
                Note = "Luyện tập hàng ngày là cần thiết!",
                Medicines = "{'name':'ahihidongok','note':'2 lần 1 ngày sau cơm','quantity':10}",
            });
            context.Bills.Add(new Bill()
            {
                CaseId = 1,
                Diagnosis = "Xuất tinh siêu sớm. 3 phút ggwp",
                Note = "Luyện tập hàng ngày là vô cùng cần thiết!",
                Medicines = "{'name':'ahihidongok','note':'3 lần 1 ngày sau cơm','quantity':100}",
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedCase(ApplicationContext context)
        {
            context.Cases.Add(new Case()
            {
                StaffId = StaffId,
                PatientId = PatientId,
                SpecialistId = 1
            });
            await context.SaveChangesAsync();
        }

        public static async Task SeedAppointment(ApplicationContext context)
        {
            context.Appointments.Add(new Appointment()
            {
                CaseId = 1,
                StaffId = StaffId,
                PatientId = PatientId,
                Time = DateTime.Parse("2019/05/22")
            });
            context.Appointments.Add(new Appointment()
            {
                CaseId = 1,
                StaffId = StaffId,
                PatientId = PatientId,
                Time = DateTime.Parse("2019/05/20"),
                Status = Status.Done
            });
            await context.SaveChangesAsync();
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

        public static async Task SeedStaff(ApplicationContext context)
        {
            var staff = new Staff()
            {
                UserId = 1,
                FacilityId = 1,
                SpecialistId = 1
            };
            context.Staffs.Add(staff);
            StaffId = staff.Id; // safer
            await context.SaveChangesAsync();
        }

        public static async Task SeedPatient(ApplicationContext context)
        {
            var patient = new Patient()
            {
                UserId = 2
            };
            context.Patients.Add(patient);
            PatientId = patient.Id; // this is safer
            await context.SaveChangesAsync();
        }
    }
}
