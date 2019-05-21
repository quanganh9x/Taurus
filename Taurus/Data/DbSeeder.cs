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
        public static void Seed(ApplicationContext context)
        {
            SeedSpecialist(context);
            SeedFacility(context);
            SeedStaff(context);
            SeedPatient(context);
            SeedAppointment(context);
            SeedBill(context);
        }

        public static void SeedBill(ApplicationContext context)
        {
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
            context.Appointments.Add(new Appointment()
            {
                
            });
            context.SaveChanges();
        }

        public static void SeedSpecialist(ApplicationContext context)
        {
            context.Specialists.Add(new Specialist()
            {
                Name = "Nha Khoa"
            });
            context.SaveChanges();
        }

        public static void SeedFacility(ApplicationContext context)
        {
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
            context.Patients.Add(new Patient()
            {
                UserId = 1,
            });
            context.SaveChanges();
           
        }
    }
}
