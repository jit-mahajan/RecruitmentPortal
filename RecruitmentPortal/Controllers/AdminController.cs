using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

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
        public async Task<ActionResult<IEnumerable<CandidateDto>>> GetAllCandidatesAsync(int pageNumber = 1)
        {
            var candidates = await _iAdminServices.GetAllCandidatesAsync(pageNumber);
            return Ok(candidates);
        }

        [HttpGet("api/getAll-recruiters")]
        public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetAllRecruitersAsync(int pageNumber = 1)
        {
            var recruiters = await _iAdminServices.GetAllRecruitersAsync(pageNumber);
            return Ok(recruiters);
        }

    }
}
