using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class JobService : IJobs
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _config;

        public JobService(IConfiguration configuration, MyDbContext context)
        {
            _config = configuration;
            _context = context;
        }

        public async Task<ActionResult> AddJobAsync(Jobs job)
        {
            Jobs jobs = new Jobs();

            jobs.JobTitle = job.JobTitle;
            jobs.Description = job.Description;
            jobs.CompanyName = job.CompanyName;
            jobs.Location = job.Location;
            jobs.PostedDate = job.PostedDate;
            jobs.JobType = job.JobType;
            jobs.Salary = job.Salary;
            jobs.Qualifications = job.Qualifications;

            try
            {
                await _context.AddAsync(jobs);
                await _context.SaveChangesAsync();

                // Return success response
                return new OkObjectResult(new { message = "Job added successfully" });
            }
            catch (Exception ex)
            {
                // Return error response
                return new ObjectResult(new { message = "An error occurred while adding the job", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        
        public async Task<ActionResult<IEnumerable<Jobs>>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _context.Jobs.ToListAsync();

                return new OkObjectResult(jobs);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while requesting jobs", error = ex.Message })
                {
                    StatusCode = 500
                };
            }

        }
        public async Task<ActionResult> DeleteJobAsync(int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    return new NotFoundObjectResult(new { message = "Enter a valid Job Id" });
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Job deleted successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while deleting the job", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }




    }
}
