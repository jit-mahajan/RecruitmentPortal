using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface ILogin
    {
        public Task<ActionResult> LoginAsync(string email, string password);

        public  Task<ActionResult> ResetPasswordAsync(string email, string newPassword);

        public int GetCurrentUserId();
        public Task<string> GetUserIdAsync(string usernameOrEmail);

        // public Task<ActionResult> ResetPasswordAsync(string userEmail, string newPassword);
    }
}
