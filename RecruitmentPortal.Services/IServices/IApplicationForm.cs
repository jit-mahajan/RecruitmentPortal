using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IApplicationForm
    {
        public Task<ActionResult> ApplyToJobAsync(JobApplicationDto jobApplicationDto);
    }
}
