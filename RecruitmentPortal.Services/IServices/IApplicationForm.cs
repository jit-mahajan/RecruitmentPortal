﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.IServices
{
    public interface IApplicationForm
    {
        public Task<ActionResult> ApplyToJobAsync(int jobId, int userId, string description);
    }
}