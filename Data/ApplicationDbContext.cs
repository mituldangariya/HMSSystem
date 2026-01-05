using HMSSystem.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;



namespace HMSSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AdminUser)
                .WithMany()
                .HasForeignKey(a => a.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Appointment)
                .WithMany(a => a.Feedbacks)
                .HasForeignKey(f => f.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Admin User
            //modelBuilder.Entity<User>().HasData(
            //    new User
            //    {
            //        FullName = "Admin",
            //        Email = "admin@appointment.com",
            //        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            //        MobileNumber = "9876543210",
            //        Role = "Admin"
            //        //IsActive = true,
            //        //CreatedDate = DateTime.Now
            //    }
            //);
        }
    }

}
