using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Models
{
    public class JobApplicationDto
    {
        public int JobApplicationId { get; set; }
        public DateTime AppliedDate { get; set; }
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }= string.Empty;

        public string JobTitle { get; set; }

    }
}
