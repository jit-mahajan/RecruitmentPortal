using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IRegistration
    {
        public Task<ActionResult> RegisterAsync(RegisterUserDto model);

        public  Task<IActionResult> RegisterRecruiter(RegisterUserDto model);

        public Task<IActionResult> UpdateRecruiter(int recruiterId, RegisterUserDto model);

       
    }
}
