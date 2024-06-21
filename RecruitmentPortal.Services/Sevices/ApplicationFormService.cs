﻿using Microsoft.AspNetCore.Mvc;
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



        public async Task<ActionResult> ApplyToMultipleJobsAsync(int userId, List<int> jobIds)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }

            var userEmail = user.Email;
            if (string.IsNullOrEmpty(userEmail))
            {
                return new BadRequestObjectResult(new { message = "User email is missing" });
            }

            var results = new List<object>();

            foreach (var jobId in jobIds)
            {
                try
                {
                    var existingApplication = await _context.JobApplications
                        .FirstOrDefaultAsync(app => app.JobId == jobId && app.UserId == userId);

                    if (existingApplication != null)
                    {
                        results.Add(new { JobId = jobId, message = "You have already applied for this job" });
                        continue;
                    }

                    var jobExists = await _context.Jobs.AnyAsync(j => j.JobId == jobId && j.IsActive);
                    if (!jobExists)
                    {
                        results.Add(new { JobId = jobId, message = "Job not found or is inactive" });
                        continue;
                    }

                    var jobApplication = new JobApplication
                    {
                        JobId = jobId,
                        UserId = userId,
                        AppliedDate = DateTime.UtcNow
                    };

                    _context.JobApplications.Add(jobApplication);
                    await _context.SaveChangesAsync();

                    // Send email notification
                    var jobTitle = (await _context.Jobs.FindAsync(jobId))?.JobTitle ?? "the job";
                    await _iEmail.SendEmailAsync(userEmail, "Job Application", $"Your job application for Role {jobTitle} submitted successfully.");

                    results.Add(new { JobId = jobId, message = "Job application submitted successfully" });
                }
                catch (Exception ex)
                {
                    results.Add(new { JobId = jobId, message = "An error occurred while adding the job", error = ex.Message });
                }
            }

            return new OkObjectResult(results);
        }


        public async Task<PaginatedList<JobApplicationDto>> GetAppliedApplicationsForRecruiterAsync(int? recruiterId, int pageNumber)
        {
            try
            {

                var query = _context.JobApplications
                    .Where(app => app.Jobs.RecruiterId == recruiterId)
                    .OrderByDescending(app => app.AppliedDate)
                    .Select(app => new JobApplicationDto
                    {
                        JobApplicationId = app.JobApplicationId,
                        AppliedDate = app.AppliedDate,
                        JobId = app.JobId,
                        UserId = app.UserId,
                        UserName = app.Users.Name,
                        JobTitle = app.Jobs.JobTitle,
                        CompanyName = app.Jobs.CompanyName
                    });

                return await PaginatedList<JobApplicationDto>.CreateAsync(query, pageNumber);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new ApplicationException("An error occurred while fetching job applications.", ex);
            }
        }

        public async Task<List<JobApplicationDto>> GetJobApplicationsAppliedByCandidateAsync(int candidateId)
        {
            var jobApplications = await _context.JobApplications
                            .Include(j => j.Jobs) 
                            .Include(j => j.Users) 
                            .Where(j => j.UserId == candidateId)
                            .OrderByDescending(j => j.AppliedDate)
                            .Select(j => new JobApplicationDto
                            {
                                JobApplicationId = j.JobApplicationId,
                                JobId = j.JobId,
                                UserId = j.UserId,
                                UserName = j.Users.Name, 
                                JobTitle = j.Jobs.JobTitle, 
                                CompanyName = j.Jobs.CompanyName,
                                AppliedDate = j.AppliedDate
                            })
                            .ToListAsync();


            return jobApplications;
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
