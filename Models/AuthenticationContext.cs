﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options):base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .HasKey(mc => new { mc.CourseID, mc.UserName });
            //Many-to-Many Relationship  - StudentCourses
            modelBuilder.Entity<StudentCourse>()
                .HasOne(m => m.Course)
                .WithMany(mc => mc.StudentCourses)
                .HasForeignKey(m => m.CourseID);
            modelBuilder.Entity<StudentCourse>()
                .HasOne(c => c.Student)
                .WithMany(mc => mc.StudentCourses)
                .HasForeignKey(c => c.UserName);


        }
    }
}