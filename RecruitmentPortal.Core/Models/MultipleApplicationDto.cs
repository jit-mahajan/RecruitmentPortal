using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Models
{
    public class MultipleApplicationDto
    {
        public int UserId { get; set; }
        public List<int> JobIds { get; set; }
    }
}
