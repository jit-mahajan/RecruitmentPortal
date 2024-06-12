using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class AdminServices
    {
        private readonly MyDbContext _context;
        public AdminServices(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync(int pageNumber)
        {
            var candidates = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Candidate"))
                .OrderByDescending(u => u.CreatedDate)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(u => new CandidateDto
                {
                    Id = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedDate = u.CreatedDate

                })
                .ToListAsync();

            return candidates;
        }

        public async Task<IEnumerable<RecruiterDto>> GetAllRecruitersAsync(int pageNumber)
        {
            var recruiters = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Recruiter"))
                .OrderByDescending(u => u.CreatedDate)
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .Select(u => new RecruiterDto
                {
                    Id = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedDate = u.CreatedDate

                })
                .ToListAsync();

            return recruiters;
        }

    }
}
