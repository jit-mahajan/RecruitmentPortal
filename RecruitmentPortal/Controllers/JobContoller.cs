using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;
using System.Security.Claims;

namespace RecruitmentPortal.Controllers
{
    public class JobContoller : ControllerBase
    {

        private readonly IJobs _iJobs;
        public JobContoller(IJobs iJobs)
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
                return Unauthorized(); // Handle case where user identity is not found
            }
            var RecruiterName = userIdClaim.Value;
            return await _iJobs.AddJobAsync(jobDto, RecruiterName);
        }


        [HttpGet("GetAllJobs")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<ActionResult<IEnumerable<Jobs>>> GetAllJobs()
        {

            return await _iJobs.GetAllJobsAsync();
        }



        [HttpDelete("delete-job/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteJob(int id)
        {
            return await _iJobs.DeleteJobAsync(id);
        }
    }

}
