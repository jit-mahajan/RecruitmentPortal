using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Entity
{
    public class Jobs
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public string JobTitle { get; set; }
        public string Description { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public string JobType { get; set; } // e.g., Full-time, Part-time, Contract

        [Required]
        public decimal? Salary { get; set; }

        [Required]
        public string Qualifications { get; set; }

        public bool IsActive { get; set; }
    }
}
