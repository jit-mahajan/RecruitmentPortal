using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IRegistration
    {
        public Task<ActionResult> RegisterAsync(Users users);
    }
}
