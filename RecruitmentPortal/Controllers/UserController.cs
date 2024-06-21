using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Controllers
{

    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUser _iUser;
        private readonly IToken _iToken;
        public UserController(IUser iUser, IToken iToken)
        {
            _iUser = iUser;
            _iToken = iToken;
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
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is missing." });
            }

            _iToken.InvalidateToken(token);
            return Ok(new { message = "Successfully logged out." });
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

        

        [HttpDelete("delete-candidate")]
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


        [HttpDelete("delete-Recruiter")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRecruiter(int id)
        {
            bool result = await _iUser.RemoveRecruiterAsync(id);
            if (result)
            {
                return Ok(new { message = "Recruiter removed successfully" });
            }
            else
            {
                return NotFound(new { message = "Recruiter not found" });
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
