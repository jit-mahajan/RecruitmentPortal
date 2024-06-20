using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IUser
    {
        public Task<ActionResult> RegisterAsync(RegisterUserDto model);

        public Task<IActionResult> RegisterRecruiter(RegisterUserDto model);

        public Task<IActionResult> UpdateRecruiter(int recruiterId, RegisterUserDto model);


        public Task<ActionResult> LoginAsync(string email, string password);

       
        public Task<ActionResult> ResetPasswordAsync(string email, string newPassword);

        public int GetCurrentUserId();

      //  Task<int?> GetUserIdAsync(string usernameOrEmail);

        Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync(int pageNumber);
        Task<IEnumerable<RecruiterDto>> GetAllRecruitersAsync(int pageNumber);

        Task<bool> RemoveCandidateAsync(int candidateID);
        Task<bool> RemoveRecruiterAsync(int recruiterID);

        Task<string> ExportAllCandidatesAsync();

       Task<string> ExportAllRecruitersAsync();

       Task<int?> GetUserIdAsync(string usernameOrEmail);

    }
}
