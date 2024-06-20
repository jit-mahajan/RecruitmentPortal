using RecruitmentPortal.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IToken
    {
        string GenerateToken(Users users);
        void InvalidateToken(string token);
        bool IsTokenValid(string token);
     

    }
}
