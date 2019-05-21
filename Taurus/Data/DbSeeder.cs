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
        public static void Seed(ApplicationContext context)
        {
            SeedSpecialist(context);
            Thread.Sleep(5000);
            SeedFacility(context);
            Thread.Sleep(5000);
            SeedStaff(context);
            Thread.Sleep(5000);
            SeedPatient(context);
            Thread.Sleep(5000);
            SeedCase(context);
            Thread.Sleep(5000);
            SeedAppointment(context);
            Thread.Sleep(5000);
            SeedBill(context);
            Thread.Sleep(5000);
        }


        public static void SeedBill(ApplicationContext context)
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
            context.SaveChanges();
        }

        public static void SeedCase(ApplicationContext context)
        {
            context.Cases.Add(new Case()
            {
                StaffId = StaffId,
                PatientId = PatientId,
                SpecialistId = 1
            });
            context.SaveChanges();
        }

        public static void SeedAppointment(ApplicationContext context)
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
            context.SaveChanges();
        }

        public static void SeedSpecialist(ApplicationContext context)
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
            context.SaveChanges();
        }

        public static void SeedFacility(ApplicationContext context)
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
            context.SaveChanges();
        }

        public static void SeedStaff(ApplicationContext context)
        {
            var staff = new Staff()
            {
                UserId = 1,
                FacilityId = 1,
                SpecialistId = 1
            };
            context.Staffs.Add(staff);
            context.SaveChanges();
            StaffId = staff.Id;
        }

        public static void SeedPatient(ApplicationContext context)
        {
            var patient = new Patient()
            {
                UserId = 2
            };
            context.Patients.Add(patient);
            context.SaveChanges();
            PatientId = patient.Id;
        }
    }
}
