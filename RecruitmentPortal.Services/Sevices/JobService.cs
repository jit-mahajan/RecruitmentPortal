using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<ActionResult> AddJobAsync(JobDto jobDto, string recruiterName)
        {

            int? recruiterId = await GetUserIdByUsernameAsync(recruiterName);

            if (recruiterId == null)
            {
                return new NotFoundObjectResult(new { message = "Recruiter not found" });
            }

            var job = new Jobs
            {
                JobTitle = jobDto.JobTitle,
                Description = jobDto.Description,
                CompanyName = jobDto.CompanyName,
                Location = jobDto.Location,
                JobType = jobDto.JobType,
                Salary = jobDto.Salary,
                Qualifications = jobDto.Qualifications,
                PostedDate = DateTime.Now,
                IsActive = true,
                RecruiterId = recruiterId,
            };


            try
            {
                await _context.AddAsync(job);
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

        public async Task<int?> GetUserIdByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            return user?.UserId;
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

        public async Task<IEnumerable<JobDto>> GetJobsOrderedByRecentAsync(int pageNumber)
        {
            var jobs = await _context.Jobs
                .OrderByDescending(j => j.PostedDate)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(j => new JobDto
                {
                    
                    JobTitle = j.JobTitle,
                    Description = j.Description,
                    CompanyName = j.CompanyName,
                    Location = j.Location,
                    JobType = j.JobType,
                    Salary  = j.Salary,
                    Qualifications = j.Qualifications
        
    })
                .ToListAsync();

            return jobs;
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
