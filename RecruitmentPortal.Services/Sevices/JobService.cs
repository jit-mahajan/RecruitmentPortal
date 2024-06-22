using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.HelperMethods;
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

            int? recruiterId = await GetIdByName.GetUserId(_context, recruiterName);

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

                return new OkObjectResult(new { message = "Job added successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while adding the job", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<ActionResult> RemoveJobAsync(int id)
        {
            try
            {
                var job = await _context.Jobs
                    .Include(j => j.JobApplications)
                    .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                {
                    return new NotFoundObjectResult(new { message = "Enter a valid Job Id" });
                }

                _context.JobApplications.RemoveRange(job.JobApplications);
                await _context.SaveChangesAsync();

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

        /*
        public async Task<int?> GetUserIdByUsernameAsync(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            return user?.UserId;
        }

        */
        public async Task<(List<JobDto> Jobs, int TotalCount)> GetJobsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Jobs
                                .Where(job => job.IsActive) 
                                .OrderByDescending(job => job.PostedDate)
                                .AsQueryable();

            int totalCount = await query.CountAsync();
            var jobs = await query.Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .Select(j => new JobDto
                                  {
                                      JobTitle = j.JobTitle,
                                      Description = j.Description,
                                      CompanyName = j.CompanyName,
                                      Location = j.Location,
                                      JobType = j.JobType,
                                      Salary = j.Salary,
                                      Qualifications = j.Qualifications
                                  })
                                  .ToListAsync();

            return (jobs, totalCount);
        }

    }
}
