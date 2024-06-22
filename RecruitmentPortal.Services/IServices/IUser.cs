using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Entity;
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
        Task<ActionResult> RegisterAsync(UsersDto model);

        Task<IActionResult> AddAdmin(UsersDto model);
        public Task<IActionResult> RegisterRecruiter(UsersDto model);

        Task<IActionResult> UpdateRecruiter(int recruiterId, UsersDto model);


        Task<ActionResult> LoginAsync(string email, string password);

       
        Task<ActionResult> ResetPasswordAsync(string email, string newPassword);

       // public int GetCurrentUserId();

        Task<int?> GetUserIdAsync(string usernameOrEmail);

        Task<IEnumerable<UsersDto>> GetAllCandidatesAsync(int pageNumber);
        Task<IEnumerable<UsersDto>> GetAllRecruitersAsync(int pageNumber);

        Task<bool> RemoveCandidateAsync(int candidateID);
        Task<bool> RemoveRecruiterAsync(int recruiterID);

        Task<string> ExportAllCandidatesAsync();

       Task<string> ExportAllRecruitersAsync();

      //B Task<int?> GetUserIdAsync(string usernameOrEmail);

    }
}
