using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IApplicationForm
    {
        Task<ActionResult> ApplyToJobAsync(JobApplicationDto jobApplicationDto);
        Task ApplyToMultipleJobsAsync(int candidateId, List<int> jobIds);

        // Task<PaginatedList<JobApplicationDto>> GetJobApplicationsAsync(int pageNumber);

        Task<List<JobApplicationDto>> GetJobApplicationsAppliedByCandidateAsync(int candidateId);
        Task<PaginatedList<JobApplicationDto>> GetApplicationsForRecruiterAsync(int? recruiterId, int pageNumber);

        Task<string> ExportCandidatesAppliedToJobsAsync();

        // Task<Stream> ExportCandidatesAppliedToJobsAsync();
    }
}
