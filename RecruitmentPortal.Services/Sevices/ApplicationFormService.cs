using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RecruitmentPortal.Services.Sevices
{
    public class ApplicationFormService
    {
        private IConfiguration _config;
        private readonly MyDbContext _context;
      //  private readonly IEmailService _emailService;

        public ApplicationFormService(IConfiguration configuration, MyDbContext context)
        {
            _config = configuration;
            _context = context;
        
        }

        /*
    
        public async Task<ActionResult> ApplyToJobAsync(int jobId, int userId, string description)
        {
            try
            {
               
                var existingApplication = await _context.JobApplications.FirstOrDefaultAsync(app => app.JobId == jobId && app.UserId == userId);  //.FirstOrDefaultAsync(app => app.JobId == jobId && app.UserId == userId);

                if (existingApplication != null)
                {
                    // If the application already exists, return a conflict response
                    return new ConflictObjectResult(new { message = "You have already applied for this job" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                var userEmail = user?.Email;
                if (userEmail != null)
                {
                   // await _emailService.SendEmailAsync(userEmail, "Job Application Submitted", "Your job application has been submitted successfully.");
                }

                // Create a new JobApplication instance
                var jobApplication = new JobApplication
                {
                    JobId = jobId,
                    UserId = userId,
                   
                    ApplicationDate = DateTime.Now,
                    Skills = j,
                    ModifiedDate = DateTime.Now,
                    IsActive = true
                };

                await _context.JobApplications.AddAsync(jobApplication);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Application submitted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here (not shown in this code)
                return new ObjectResult(new { message = "An error occurred while submitting the application", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
      */
     
    }
}
