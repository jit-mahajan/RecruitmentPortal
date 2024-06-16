using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class AdminServices : IAdminServices
    {
        private readonly MyDbContext _context;
        public AdminServices(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync(int pageNumber)
        {
            try
            {
                var candidates = await _context.Users
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
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new ApplicationException("An error occurred while fetching candidates.", ex);
            }
        }

        public async Task<IEnumerable<RecruiterDto>> GetAllRecruitersAsync(int pageNumber)
        {
            try
            {
                var recruiters = await _context.Users
                    .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Recruiter"))
                     .OrderByDescending(u => u.CreatedDate)
                     .Skip((pageNumber - 1) * 10)
                     .Take(10)
                     .Select(u => new RecruiterDto
                     {
                         //Id = u.UserId,
                         Name = u.Name,
                         Email = u.Email,
                         CreatedDate = u.CreatedDate

                     })
                     .ToListAsync();

                return recruiters;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                throw new ApplicationException("An error occurred while fetching recruiters.", ex);
            }
        }
    }

}
