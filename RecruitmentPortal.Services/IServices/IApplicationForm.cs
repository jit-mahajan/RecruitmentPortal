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

        Task ApplyToMultipleJobsAsync(List<JobApplicationDto> jobApplicationDtos);

        Task<(List<JobApplicationDto> Applications, int TotalCount)> GetJobApplicationsAppliedByCandidateAsync(int candidateId, int pageNumber, int pageSize);
  
        Task<(List<JobApplicationDto> Applications, int TotalCount)> GetApplicantsForRecruiterJobsAsync(int recruiterId, int pageNumber, int pageSize);
        Task<string> ExportCandidatesAppliedToJobsAsync();

    }
}
