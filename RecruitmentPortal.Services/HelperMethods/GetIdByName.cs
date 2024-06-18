using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.HelperMethods
{
    public class GetIdByName
    {
        public static async Task<int?> GetUserId(MyDbContext context, string usernameOrEmail)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Name == usernameOrEmail || u.Email == usernameOrEmail);        //.FirstOrDefaultAsync(u => u.Name == usernameOrEmail || u.Email == usernameOrEmail);
            return user?.UserId; 
        }
    }
}
