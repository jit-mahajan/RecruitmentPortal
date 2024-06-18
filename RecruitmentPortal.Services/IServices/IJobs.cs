using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IJobs
    {
         Task<ActionResult> AddJobAsync(JobDto jobDto, string recruiterName);
         Task<IEnumerable<JobDto>> GetJobsOrderedByRecentAsync(int pageNumber);
       //  Task<ActionResult<IEnumerable<Jobs>>> GetAllJobsAsync();


        Task<ActionResult> RemoveJobAsync(int id);





    }
}
