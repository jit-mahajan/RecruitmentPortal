using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public string JobType { get; set; } 

        [Required]
        public decimal? Salary { get; set; }

        [Required]
        public string Qualifications { get; set; }

        public bool IsActive { get; set; }

        public int? RecruiterId { get; set; }
        public Users Recruiter { get; set; }           //null

        public ICollection<JobApplication> JobApplications { get; set; }    //null
    }
}
