using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.HelperMethods;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class RecruiterService :IRecruiter
    {
        private readonly MyDbContext _context;

        public RecruiterService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<JobApplicationDto>> GetApplicationsForRecruiterAsync(int recruiterId, int pageNumber)
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
                        JobTitle = app.Jobs.JobTitle
                    });

                return await PaginatedList<JobApplicationDto>.CreateAsync(query, pageNumber);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new ApplicationException("An error occurred while fetching job applications.", ex);
            }
        }
    }
}
