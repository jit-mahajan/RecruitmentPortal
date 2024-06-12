using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Entity
{
    public class UserRole
    {
        public int UserId { get; set; }
        public Users User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
