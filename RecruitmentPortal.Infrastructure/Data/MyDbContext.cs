using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace RecruitmentPortal.Infrastructure.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }
      
        public DbSet<Users> Users { get; set; }
        public DbSet<Jobs> Jobs { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Job entity
            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            });
        }


        }
    }

  
