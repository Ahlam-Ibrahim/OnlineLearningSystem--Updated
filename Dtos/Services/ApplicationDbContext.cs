using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineLearningSystem.Models;


namespace OnlineLearningSystem.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
            public DbSet<Course> Courses { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<StudentCourse> StudentCourses { get; set; }
            public DbSet<Section> Sections { get; set; }
            public DbSet<Video> Videos { get; set; }
            public DbSet<CourseCategory> CourseCategories { get; set; }
            

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //enum classes

            modelBuilder
            .Entity<Course>()
            .Property(p => p.Location)
            .HasConversion(
                v => v.ToString(),
                v => (Location)Enum.Parse(typeof(Location), v));

            modelBuilder
            .Entity<StudentCourse>()
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
            modelBuilder.Entity<StudentCourse>()
                .HasKey(mc => new { mc.CourseID, mc.StudentID });
            //Many-to-Many Relationship  - StudentCourses
            modelBuilder.Entity<StudentCourse>()
                .HasOne(m => m.Course)
                .WithMany(mc => mc.StudentCourses)
                .HasForeignKey(m => m.CourseID);
            modelBuilder.Entity<StudentCourse>()
                .HasOne(c => c.Student)
                .WithMany(mc => mc.StudentCourses)
                .HasForeignKey(c => c.StudentID);


        }
    }
}