﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Entity
{
    public class JobApplication
    {
        [Key]
        public int JobApplicationId { get; set; }
        public DateTime AppliedDate { get; set; }

        public int JobId { get; set; }
        public Jobs Jobs { get; set; }

        public int UserId { get; set; }
        public Users Users { get; set; }
    }
}
