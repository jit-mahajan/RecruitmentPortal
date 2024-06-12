using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Controllers
{
    public class RegisterController : Controller
    {
        IRegistration _registrationService;
        private readonly MyDbContext  _context;
        public int UserId;
        public RegisterController (IRegistration registrationService, MyDbContext context)
        {
            _registrationService = registrationService;
            _context = context;
        }


        [HttpPost("api/register")]
        public async Task<ActionResult> Register(RegisterUserDto model)
        {
            return await _registrationService.RegisterAsync(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api/register-recruiter")]

        public async Task<IActionResult> RegisterRecruiter(RegisterUserDto model)
        {
            return await _registrationService.RegisterRecruiter(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/update-recruiter/{recruiterId}")]
        public async Task<IActionResult> UpdateRecruiter(int recruiterId, RegisterUserDto model)
        {
            return await _registrationService.UpdateRecruiter(recruiterId, model);
        }


    }
}
