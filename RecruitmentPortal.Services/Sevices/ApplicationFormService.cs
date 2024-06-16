using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.Email;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RecruitmentPortal.Services.Sevices
{
    public class ApplicationFormService : IApplicationForm
    {
        private IConfiguration _config;
        private readonly MyDbContext _context;
        private readonly IEmail _iEmail;

        public ApplicationFormService(IConfiguration configuration, MyDbContext context, IEmail iEmail)
        {
            _config = configuration;
            _context = context;
            _iEmail = iEmail;

        }


    
        public async Task<ActionResult> ApplyToJobAsync(JobApplicationDto jobApplicationDto)
        {
            try
            {
                // Check if the user has already applied for this job
                var existingApplication = await _context.JobApplications
                    .FirstOrDefaultAsync(app => app.JobId == jobApplicationDto.JobId && app.UserId == jobApplicationDto.UserId);

                if (existingApplication != null)
                {
                    // If the application already exists, return a conflict response
                    return new ConflictObjectResult(new { message = "You have already applied for this job" });
                }

                // Optionally: Check if the job exists and is active
                var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobApplicationDto.JobId && j.IsActive);
                if (!jobExists)
                {
                    return new NotFoundObjectResult(new { message = "Job not found or is inactive" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == jobApplicationDto.UserId);
                var userEmail = user?.Email;
                if (userEmail != null)
                {
                     
                     await _iEmail.SendEmailAsync(userEmail, "Job Application Submitted", "Your job application has been submitted successfully.");
                }

                var jobApplication = new JobApplication
                {
                    JobId = jobApplicationDto.JobId,
                    UserId = jobApplicationDto.UserId,
                    AppliedDate = DateTime.Now 
                };

                _context.JobApplications.Add(jobApplication);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Job application submitted successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while adding the job", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
            
        }
     
    }
}
