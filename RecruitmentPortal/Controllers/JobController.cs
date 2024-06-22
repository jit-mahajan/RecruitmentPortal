using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;
using System.Security.Claims;

namespace RecruitmentPortal.Controllers
{
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {

        private readonly IJobs _iJobs;
        public JobController(IJobs iJobs)
        {
            _iJobs = iJobs;
        }


        [HttpPost("add-job")]
        [Authorize(Roles = "Recruiter")]
        public async Task<ActionResult> AddJob(JobDto jobDto)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); 
            }
            var RecruiterName = userIdClaim.Value;
            return await _iJobs.AddJobAsync(jobDto, RecruiterName);
        }



        
        [HttpGet("getjobs-byRecent")]
        [Authorize(Roles = "Candidate,Admin,Recruiter")]
        public async Task<IActionResult> GetJobsOrderedByRecent(int pageNumber = 1, int pageSize =10)
        {
            try
            {
                var jobs = await _iJobs.GetJobsAsync(pageNumber, pageSize);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching jobs", error = ex.Message });
            }
        }


        [HttpDelete("delete-job/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveJob(int id)
        {
            return await _iJobs.RemoveJobAsync(id);
        }


    }

}
