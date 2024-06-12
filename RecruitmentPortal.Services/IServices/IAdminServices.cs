using RecruitmentPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IAdminServices
    {
        Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync(int pageNumber);
        Task<IEnumerable<RecruiterDto>> GetAllRecruitersAsync(int pageNumber);
    }
}
