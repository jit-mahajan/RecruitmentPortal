using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IJobs
    {
        public Task<ActionResult> AddJobAsync(Jobs job);
        Task<ActionResult<IEnumerable<Jobs>>> GetAllJobsAsync();
        Task<ActionResult> DeleteJobAsync(int id);


    }
}
