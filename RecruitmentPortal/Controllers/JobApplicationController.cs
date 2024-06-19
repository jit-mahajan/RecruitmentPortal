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
        public async Task<IActionResult> ApplyToMultipleJobs([FromBody] MultipleApplicationDto request)
        {
            await _iApplicationForm.ApplyToMultipleJobsAsync(request.UserId, request.JobIds);
            return Ok();
        }


        [HttpGet("candidate/{candidateId}/recentApplications")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<List<JobApplicationDto>>> GetRecentJobApplications(int candidateId)
        {
            var jobApplications = await _iApplicationForm.GetJobApplicationsAppliedByCandidateAsync(candidateId);
            return Ok(jobApplications);
        }


        /*
        [HttpGet("api/all-jobApplications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetJobApplications(int pageNumber = 1)
        {
            var jobApplications = await _iApplicationForm.GetJobApplicationsAsync(pageNumber);
            return Ok(jobApplications);
        }
        */

        [HttpGet("recruiter/{recruiterId}/appliedJobApplications")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult<PaginatedList<JobApplicationDto>>> GetApplicationsForRecruiter(int recruiterId, int pageNumber = 1)
        {

            var applications = await _iApplicationForm.GetAppliedApplicationsForRecruiterAsync(recruiterId, pageNumber);
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
