using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seeding DB with Roles and the admin account
            var adminRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(new List<IdentityRole>
            {
                new IdentityRole {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Mentor",
                    NormalizedName = "MENTOR"
                },
                new IdentityRole {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Student",
                    NormalizedName = "STUDENT"
                },
            });

            modelBuilder
            .Entity<Course>()
            .Property(p => p.Location)
            .HasConversion(
                v => v.ToString(),
                v => (Location)Enum.Parse(typeof(Location), v));

            modelBuilder
            .Entity<Enrollment>()
            .Property(p => p.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v));


            //key - CourseCategory
            modelBuilder.Entity<CourseCategory>()
                .HasKey(cc => new { cc.CategoryID, cc.CourseID });
            //Many-to-Many Relationship  - CourseCategory
            modelBuilder.Entity<CourseCategory>()
                .HasOne(c => c.Category)
                .WithMany(cc => cc.CourseCategories)
                .HasForeignKey(c => c.CategoryID);
            modelBuilder.Entity<CourseCategory>()
                .HasOne(c => c.Course)
                .WithMany(cc => cc.CourseCategories)
                .HasForeignKey(c => c.CourseID);

            //key - StudentCourses
            modelBuilder.Entity<Enrollment>()
                .HasKey(mc => new { mc.CourseID, mc.StudentID });
            //Many-to-Many Relationship  - StudentCourses
            modelBuilder.Entity<Enrollment>()
                .HasOne(m => m.Course)
                .WithMany(mc => mc.Enrollments)
                .HasForeignKey(m => m.CourseID);
            modelBuilder.Entity<Enrollment>()
                .HasOne(c => c.Student)
                .WithMany(mc => mc.Enrollments)
                .HasForeignKey(c => c.StudentID);


        }
    }
}
