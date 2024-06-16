using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Email
{
    public interface IEmail
    {
        public Task SendEmailAsync(string to, string subject, string body);
    }
}
