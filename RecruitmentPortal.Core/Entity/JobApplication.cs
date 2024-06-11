using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Entity
{
    public class JobApplication
    {
        public int JobApplicationId { get; set; } 
        public int JobId { get; set; } 
        public int UserId { get; set; } 
        public DateTime ApplicationDate { get; set; }

        public string Skills   { get; set; }

        public bool IsActive { get; set; }

        public Jobs Job { get; set; }
        public Users User { get; set; }
    }
}
