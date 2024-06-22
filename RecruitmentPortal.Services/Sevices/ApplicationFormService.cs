using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.Email;
using RecruitmentPortal.Services.HelperMethods;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var existingApplication = await _context.JobApplications
                    .FirstOrDefaultAsync(app => app.JobId == jobApplicationDto.JobId && app.UserId == jobApplicationDto.UserId);

                if (existingApplication != null)
                {
                    return new ConflictObjectResult(new { message = "You have already applied for this job" });
                }

                var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobApplicationDto.JobId && j.IsActive);
                if (!jobExists)
                {
                    return new NotFoundObjectResult(new { message = "Job not found or is inactive" });
                }


                var jobTitle = jobApplicationDto.JobTitle;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == jobApplicationDto.UserId);
                var userEmail = user?.Email;
                if (userEmail != null)
                {

                    await _iEmail.SendEmailAsync(userEmail, "Job Application", $"Your job application for Role {jobTitle} submitted successfully.");
                }

                var jobApplication = new JobApplication
                {
                    JobId = jobApplicationDto.JobId,
                    UserId = jobApplicationDto.UserId,
                    UserName = jobApplicationDto.UserName,
                    JobTitle = jobApplicationDto.JobTitle,
                    CompanyName = jobApplicationDto.CompanyName,
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



        public async Task ApplyToMultipleJobsAsync(List<JobApplicationDto> jobApplicationDtos)
        {
            foreach (var jobApplicationDto in jobApplicationDtos)
            {
                var userId = jobApplicationDto.UserId;
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException($"User not found for UserId: {userId}");
                }

                var userEmail = user.Email;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new ArgumentException($"Email not found for User with UserId: {userId}");
                }

                try
                {
                    var existingApplication = await _context.JobApplications
                        .FirstOrDefaultAsync(app => app.JobId == jobApplicationDto.JobId && app.UserId == userId);

                    if (existingApplication != null)
                    {

                        continue;
                    }

                    var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobApplicationDto.JobId && j.IsActive);
                    if (!jobExists)
                    {
                        throw new ArgumentException($"Job not found or is inactive for JobId: {jobApplicationDto.JobId}");
                    }

                    var job = await _context.Jobs.FindAsync(jobApplicationDto.JobId);
                    if (job == null)
                    {
                        throw new ArgumentException($"Job not found for JobId: {jobApplicationDto.JobId}");
                    }

                    var jobApplication = new JobApplication
                    {
                        JobId = jobApplicationDto.JobId,
                        UserId = userId,
                        UserName = jobApplicationDto.UserName,
                        JobTitle = job.JobTitle,
                        CompanyName = job.CompanyName,
                        AppliedDate = DateTime.UtcNow
                    };

                    _context.JobApplications.Add(jobApplication);
                    await _context.SaveChangesAsync();

                   
                    await _iEmail.SendEmailAsync(userEmail, "Job Application", $"Your job application for Role {job.JobTitle} submitted successfully");
                }
                catch (Exception ex)
                {
                    
                    throw new Exception($"Failed to apply for JobId: {jobApplicationDto.JobId}", ex);
                }
            }
        }

        public async Task<(List<JobApplicationDto> Applications, int TotalCount)> GetJobApplicationsAppliedByCandidateAsync(int candidateId, int pageNumber, int pageSize)
        {
            // Query to get applications by the candidate
            var query = _context.JobApplications
                                .Where(app => app.UserId == candidateId)
                                .Include(app => app.Jobs)
                                .OrderByDescending(app => app.AppliedDate)
                                .AsQueryable();

            // Get total count of applications
            int totalCount = await query.CountAsync();

            // Get paginated applications
            var applications = await query.Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();

            // Manually map to JobApplicationDto
            var applicationDtos = applications.Select(app => new JobApplicationDto
            {
                JobId = app.JobId,
                UserId = app.UserId,
                UserName = app.Users.Name,  // Assuming Users property is included and has a Name field
                JobTitle = app.Jobs.JobTitle,
                CompanyName = app.Jobs.CompanyName,
                AppliedDate = app.AppliedDate
            }).ToList();

            return (applicationDtos, totalCount);
        }

        public async Task<(List<JobApplicationDto> Applications, int TotalCount)> GetApplicantsForRecruiterJobsAsync(int recruiterId, int pageNumber, int pageSize)
        {
 
            var recruiterJobs = _context.Jobs
                                        .Where(job => job.RecruiterId == recruiterId)
                                        .Select(job => job.JobId)
                                        .AsQueryable();

            var query = _context.JobApplications
                                .Where(app => recruiterJobs.Contains(app.JobId))
                                .Include(app => app.Jobs)
                                .Include(app => app.Users)
                                .OrderByDescending(app => app.AppliedDate)
                                .AsQueryable();

            int totalCount = await query.CountAsync();

            var applications = await query.Skip((pageNumber - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();

            var applicationDtos = applications.Select(app => new JobApplicationDto
            {
                JobId = app.JobId,
                UserId = app.UserId,
                UserName = app.Users.Name,
                JobTitle = app.Jobs.JobTitle,
                CompanyName = app.Jobs.CompanyName,
                AppliedDate = app.AppliedDate
            }).ToList();

            return (applicationDtos, totalCount);
        }
        public async Task<string> ExportCandidatesAppliedToJobsAsync()
        {
            var jobApplications = await _context.JobApplications
                .Include(ja => ja.Users)
                .Include(ja => ja.Jobs)
                .ToListAsync();

            string filePath = ExportJobApplicationsToExcel(jobApplications, "CandidatesAppliedToJobs");
            return filePath;
        }

        private string ExportJobApplicationsToExcel(List<JobApplication> data, string sheetName)
        {
            var fileInfo = new FileInfo($"{sheetName}.xlsx");
            using (var package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells[1, 1].Value = "JobApplicationId";
                worksheet.Cells[1, 2].Value = "UserId";
                worksheet.Cells[1, 3].Value = "User Name";
                worksheet.Cells[1, 4].Value = "JobId";
                worksheet.Cells[1, 5].Value = "Job Title";
                worksheet.Cells[1, 6].Value = "Description";
                worksheet.Cells[1, 7].Value = "AppliedDate";

                for (int i = 0; i < data.Count; i++)
                {
                    JobApplication application = data[i];
                    worksheet.Cells[i + 2, 1].Value = application.JobApplicationId;
                    worksheet.Cells[i + 2, 2].Value = application.UserId;
                    worksheet.Cells[i + 2, 3].Value = application.Users.Name;
                    worksheet.Cells[i + 2, 4].Value = application.JobId;
                    worksheet.Cells[i + 2, 5].Value = application.Jobs.JobTitle;
                    worksheet.Cells[i + 2, 6].Value = application.Jobs.Description;
                    worksheet.Cells[i + 2, 7].Value = application.AppliedDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                package.Save();
            }
            return fileInfo.FullName;
        }
       





    }



}
