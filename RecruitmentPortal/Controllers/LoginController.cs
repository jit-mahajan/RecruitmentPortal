using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Controllers
{


    public class LoginController : ControllerBase
    {
        ILogin _loginService;
        private readonly MyDbContext _context;
        public int UserId;
        public LoginController(ILogin loginService, MyDbContext context)
        {

            _loginService = loginService;
            UserId = loginService.GetCurrentUserId();
            _context = context;
        }


        [HttpPost("api/login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var loginResult = await _loginService.LoginAsync(email, password);
                if (loginResult != null)
                {
                    return Ok("Login Successful");
                }
                else
                {
                   
                    return Unauthorized(new { message = "Invalid email or password" });
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the job", error = ex.Message });
            }
        }

        [HttpPost("api/reset-password")]
        public async Task<ActionResult> ResetPassword(string userEmail, string newPassword)
        {
            return await _loginService.ResetPasswordAsync(userEmail, newPassword);
        }

    }
}
