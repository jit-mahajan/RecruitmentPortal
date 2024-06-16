using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Controllers
{
    public class CandidateController : ControllerBase
    {


        private readonly IJobs _iJobs;

        private readonly IApplicationForm _iApplicationForm;
        public CandidateController(IJobs iJobs, IApplicationForm iApplicationForm)
        {
            _iJobs = iJobs;
            _iApplicationForm = iApplicationForm;
        }

        [HttpGet("api/getjobs-byRecent")]
        public async Task<IActionResult> GetJobsOrderedByRecent(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var jobs = await _iJobs.GetJobsOrderedByRecentAsync(pageNumber);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching jobs", error = ex.Message });
            }
        }


        [HttpPost("apply-to-job")]
        public async Task<IActionResult> ApplyToJobAsync(JobApplicationDto jobApplicationDto)
        {
            return await _iApplicationForm.ApplyToJobAsync(jobApplicationDto);
        }
    }
}
