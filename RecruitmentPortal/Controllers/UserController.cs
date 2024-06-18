using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;

namespace RecruitmentPortal.Controllers
{

    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUser _iUser;
        public UserController(IUser iUser)
        {
            _iUser = iUser;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserDto model)
        {
            return await _iUser.RegisterAsync(model);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {

            return await _iUser.LoginAsync(email, password);
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(string userEmail, string newPassword)
        {
            return await _iUser.ResetPasswordAsync(userEmail, newPassword);
        }

      
        [HttpGet("getAll-candidates")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CandidateDto>>> GetAllCandidatesAsync(int pageNumber = 1)
        {
            var candidates = await _iUser.GetAllCandidatesAsync(pageNumber);
            return Ok(candidates);
        }

        

        [HttpDelete("delete-candidate/{candidateId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveCandidate(int id)
        {
            bool result = await _iUser.RemoveCandidateAsync(id);
            if (result)
            {
                return Ok(new { message = "Candidate removed successfully" });
            }
            else
            {
                return NotFound(new { message = "Candidate not found" });
            }
        }
        
        [HttpPost("register-recruiter")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> RegisterRecruiter(RegisterUserDto model)
        {
            return await _iUser.RegisterRecruiter(model);
        }


        [HttpPut("update-recruiter/{recruiterId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRecruiter(int recruiterId, RegisterUserDto model)
        {
            return await _iUser.UpdateRecruiter(recruiterId, model);
        }


        [HttpGet("getAll-recruiters")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetAllRecruitersAsync(int pageNumber = 1)
        {
            var recruiters = await _iUser.GetAllRecruitersAsync(pageNumber);
            return Ok(recruiters);
        }


        [HttpDelete("delete-Recruiter/{RecruiterId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRecruiter(int id)
        {
            bool result = await _iUser.RemoveRecruiterAsync(id);
            if (result)
            {
                return Ok(new { message = "Candidate removed successfully" });
            }
            else
            {
                return NotFound(new { message = "Candidate not found" });
            }
        }


        [HttpGet("exportAll-candidates")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAllCandidates()
        {
            string filePath = await _iUser.ExportAllCandidatesAsync();
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllCandidates.xlsx");
        }


        [HttpGet("exportAll-recruiters")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAllRecruiters()
        {
            string filePath = await _iUser.ExportAllRecruitersAsync();    
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllRecruiters.xlsx");
        }

    }
}
