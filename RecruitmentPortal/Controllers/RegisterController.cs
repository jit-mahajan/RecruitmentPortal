using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
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
        public async Task<ActionResult> Register(Users users)
        {
            return await _registrationService.RegisterAsync(users);
        }

    }
}
