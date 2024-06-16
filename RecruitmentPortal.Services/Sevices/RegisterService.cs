using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
     public class RegisterService : IRegistration
    {

       // private IConfiguration _config;
        //   private readonly List<string> _blacklistedTokens;
        //  private readonly IHttpContextAccessor _httpContextAccessor;

        /*
        public RegisterService(IConfiguration configuration, MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {

            _config = configuration;
            _context = context;
            _blacklistedTokens = new List<string>();
            _httpContextAccessor = httpContextAccessor;
        }
*/
        private readonly MyDbContext _context;
        public RegisterService(MyDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> RegisterAsync(RegisterUserDto model)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return new ConflictObjectResult(new { message = "Email already exists" });
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return new BadRequestObjectResult(new { message = "Passwords do not match" });
                }

                if (!IsPasswordValid(model.Password))
                {
                    return new BadRequestObjectResult(new { message = "Password must be at least 8 characters long and contain at least one uppercase letter, one special character, and one numeric character." });
                }

                string hashedPassword = HelperMethods.PasswordHelper.HashPassword(model.Password);

                var user = new Users
                {
                    Name = model.Name,
                    Gender = model.Gender,
                    ContactNo = model.ContactNo,
                    Email = model.Email,
                    Password = hashedPassword,
                    IsActive = true,

                    UserRoles = new List<UserRole>
                    {
                        new UserRole
                        {
                            RoleId = _context.Roles.Single(r => r.RoleName == "Candidate").RoleId
                        }
                    }
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "User Created Successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception here (actual logging not shown)
                return new ObjectResult(new { message = "An error occurred while creating the user", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<IActionResult> RegisterRecruiter(RegisterUserDto model)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return new ConflictObjectResult(new { message = "Email already exists" });
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return new BadRequestObjectResult(new { message = "Passwords do not match" });
                }

                if (!IsPasswordValid(model.Password))
                {
                    return new BadRequestObjectResult(new { message = "Password does not meet complexity requirements" });
                }

                string hashedPassword = HelperMethods.PasswordHelper.HashPassword(model.Password);

                var user = new Users
                {
                    Name = model.Name,
                    Gender = model.Gender,
                    ContactNo = model.ContactNo,
                    Email = model.Email,
                    Password = hashedPassword,
                    IsActive = true,
                    UserRoles = new[]
                    {
                        new UserRole
                        {
                            RoleId = _context.Roles.Single(r => r.RoleName == "Recruiter").RoleId
                        }
                    }
                
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Recruiter created successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while creating the recruiter", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> UpdateRecruiter(int recruiterId, RegisterUserDto model)
        {
            try
            {
                var recruiter = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.UserId == recruiterId);

                if (recruiter == null)
                {
                    return new NotFoundObjectResult(new { message = "Recruiter not found" });
                }

                // Only allow updates by admin
                if (!recruiter.UserRoles.Any(ur => ur.Role.RoleName == "Admin"))
                {
                    return new ForbidResult();
                }

                // Update recruiter properties
                recruiter.Name = model.Name;
                recruiter.Gender = model.Gender;
                recruiter.ContactNo = model.ContactNo;
                recruiter.Email = model.Email;

                // Password update logic can be added if needed

                _context.Users.Update(recruiter);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Recruiter updated successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while updating the recruiter", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public bool IsPasswordValid(string? password)
        {

            if (password == null)
            {
                return false;
            }
            
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";

            return Regex.IsMatch(password, pattern);
        }

       
     }
}
