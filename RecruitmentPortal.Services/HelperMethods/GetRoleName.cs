using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.HelperMethods
{
    public class GetRoleName
    {
        public static async Task<string> GetUserRoleAsync(int userId, MyDbContext _context)
        {
            var userRole = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.RoleId,
                      (ur, r) => r.RoleName)
                .FirstOrDefaultAsync();

            return userRole;
        }
    }
}
