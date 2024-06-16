using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Services.HelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IRecruiter
    {
        Task<PaginatedList<JobApplicationDto>> GetApplicationsForRecruiterAsync(int recruiterId, int pageNumber);
    }
}
