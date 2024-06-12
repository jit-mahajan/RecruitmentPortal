using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Entity
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Mobile number must be between 10 and 15 characters.")]
        public int ContactNo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public ICollection<UserRole> UserRoles { get; set; }  = new List<UserRole>();

        public bool IsActive { get; set; }

    }
}
