﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OfficeManagement.Areas.Identity.Data;
using OfficeManagement.Models;

namespace OfficeManagement.Data
{
    public class OfficeContext : IdentityDbContext<OfficeUser>
    {
        public OfficeContext(DbContextOptions<OfficeContext> options)
            : base(options)
        {
        }

        // DbSets for other entities in the system
        public DbSet<Staff> Personnel { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BugTracking> Bugs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<Profile> Summary { get; set; }

        // DbSet for OfficeUser (custom Identity User)
        public DbSet<OfficeUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Custom table mappings
            modelBuilder.Entity<Staff>().ToTable("staff");
            //modelBuilder.Entity<Activity>().ToTable("activity");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Assignment>().ToTable("assignment");
            modelBuilder.Entity<Project>().ToTable("project");
            modelBuilder.Entity<Message>().ToTable("message");
            modelBuilder.Entity<Mail>().ToTable("mail");
            modelBuilder.Entity<Profile>().ToTable("users"); // This is the users table for profiles
            modelBuilder.Entity<BugTracking>().ToTable("bug");

            // Configuring OfficeUser entity
            modelBuilder.Entity<OfficeUser>(entity =>
            {
                // Ensure correct table mapping
                entity.ToTable("OfficeUsers");

                // Configure properties
                entity.Property(e => e.FirstName).HasMaxLength(255);
                entity.Property(e => e.LastName).HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.UserRole).HasMaxLength(255);
            });
        }
    }
}


