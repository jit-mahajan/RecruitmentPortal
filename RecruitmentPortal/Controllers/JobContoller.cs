using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

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
        [Authorize(Roles = "Recruter")]
        public async Task<ActionResult> AddJob(Jobs job)
        {
            return await _iJobs.AddJobAsync(job);
        }


        [HttpGet("GetAllJobs")]
        [Authorize(Roles = "Admin,Recruter")]
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
