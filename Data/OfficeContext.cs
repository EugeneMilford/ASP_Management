using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OfficeManagement.Models;

namespace OfficeManagement.Data
{
    public class OfficeContext : DbContext
    {
        public OfficeContext(DbContextOptions<OfficeContext> options)
            : base(options)
        {
        }

        public DbSet<Staff> Personnel { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BugTracking> Bugs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Staff>().ToTable("staff");
            modelBuilder.Entity<Event>().ToTable("event");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Assignment>().ToTable("assignment");
            modelBuilder.Entity<Project>().ToTable("project");
            modelBuilder.Entity<BugTracking>().HasNoKey().ToTable("bug");
        }
    }
}


