using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;

namespace RecruitmentPortal.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _iAdminServices;

        public AdminController(IAdminServices iAdminServices)
        {
            _iAdminServices = iAdminServices;
        }

       
        [HttpGet("api/getAll-candidates")]
        public async Task<ActionResult<IEnumerable<CandidateDto>>> GetPaginatedCandidates(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var candidates = await _iAdminServices.GetAllCandidatesAsync(pageNumber);
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching candidates", error = ex.Message });
            }
        }

        [HttpGet("api/getAll-recruiters")]
        public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetPaginatedRecruiters(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var recruiters = await _iAdminServices.GetAllRecruitersAsync(pageNumber);
                return Ok(recruiters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching recruiters", error = ex.Message });
            }
        }

    }
}
