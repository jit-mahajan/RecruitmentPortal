using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.HelperMethods;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterController : ControllerBase
    {

        private readonly IRecruiter _iRecruiter;

        public RecruiterController(IRecruiter iRecruiter)
        {
            _iRecruiter = iRecruiter;
        }

        [HttpGet("api/recruiter/applications")]
        public async Task<ActionResult<PaginatedList<JobApplicationDto>>> GetApplicationsForRecruiter(int recruiterId, int pageNumber = 1)
        {
            try
            {
                var applications = await _iRecruiter.GetApplicationsForRecruiterAsync(recruiterId, pageNumber);
                return Ok(applications);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }
    }
}
