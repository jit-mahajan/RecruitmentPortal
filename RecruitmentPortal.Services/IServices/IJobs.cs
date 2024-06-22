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

        Task<(List<JobDto> Jobs, int TotalCount)> GetJobsAsync(int pageNumber, int pageSize);
        Task<ActionResult> RemoveJobAsync(int id);





    }
}
