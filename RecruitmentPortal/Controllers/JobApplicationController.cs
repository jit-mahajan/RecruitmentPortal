using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.HelperMethods;
using RecruitmentPortal.Services.IServices;
using System.Security.Claims;

namespace RecruitmentPortal.Controllers
{
    [Route("api/[controller]")]
    public class JobApplicationController : ControllerBase
    {


        private readonly IApplicationForm _iApplicationForm;
        private readonly IUser _iUser;

        public JobApplicationController(IApplicationForm iApplicationForm, IUser iUser)
        {
            _iApplicationForm = iApplicationForm;
            _iUser = iUser;
        }

        [HttpPost("apply-to-job")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> ApplyToJobAsync(JobApplicationDto jobApplicationDto)
        {
            return await _iApplicationForm.ApplyToJobAsync(jobApplicationDto);
        }



        [HttpPost("apply-to-multipleJobs")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> ApplyToMultipleJobs([FromBody] List<JobApplicationDto> jobApplicationDtos)
        {
            await _iApplicationForm.ApplyToMultipleJobsAsync(jobApplicationDtos);
            return Ok();
        }


        [HttpGet("candidate/{candidateId}/recentApplications")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<JobApplicationDto>>> GetRecentJobApplications(int candidateId, int pageNumber, int pageSize)
        {
            var jobApplications = await _iApplicationForm.GetJobApplicationsAppliedByCandidateAsync(candidateId, pageNumber, pageSize);           //GetJobApplicationsAppliedByCandidateAsync(candidateId);
            return Ok(jobApplications);
        }


      
        [HttpGet("recruiter/{recruiterId}/appliedJobApplications")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetApplicantsForRecruiterJobs(int recruiterId, int pageNumber = 1, int pageSize = 10)
        {

            var applications = await _iApplicationForm.GetApplicantsForRecruiterJobsAsync(recruiterId, pageNumber, pageSize);
            return Ok(applications);

        }

        [HttpGet("export/job-applications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportCandidatesAppliedToJobs()
        {
            string filePath = await _iApplicationForm.ExportCandidatesAppliedToJobsAsync();
            return PhysicalFile(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CandidatesAppliedToJobs.xlsx");
        }

    }
}
