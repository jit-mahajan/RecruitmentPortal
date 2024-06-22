using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Core.Models;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Services.HelperMethods;
using RecruitmentPortal.Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class UserService : IUser
    {

        private IConfiguration _config;
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IToken _itoken;
        public UserService (IConfiguration configuration, MyDbContext context, IHttpContextAccessor httpContextAccessor, IToken itoken)
        {
            _config = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _itoken = itoken;   
        }
        public async Task<ActionResult> RegisterAsync(UsersDto model)
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

                string hashedPassword = PasswordHelper.HashPassword(model.Password);

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
                return new ObjectResult(new { message = "An error occurred while creating the user", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }


        public async Task<IActionResult> AddAdmin(UsersDto model)
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

                string hashedPassword = PasswordHelper.HashPassword(model.Password);

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
                            RoleId = _context.Roles.Single(r => r.RoleName == "Admin").RoleId
                        }
                    }

                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Admin created successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while creating the recruiter", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }


        public async Task<IActionResult> RegisterRecruiter(UsersDto model)
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

                string hashedPassword = PasswordHelper.HashPassword(model.Password);

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

        public async Task<IActionResult> UpdateRecruiter(int recruiterId, UsersDto model)
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

             
                if (!recruiter.UserRoles.Any(ur => ur.Role.RoleName == "Admin"))
                {
                    return new ForbidResult();
                }

                recruiter.Name = model.Name;
                recruiter.Gender = model.Gender;
                recruiter.ContactNo = model.ContactNo;
                recruiter.Email = model.Email;

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


        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await AuthenticateUserAsync(email, password);

                if (user != null)
                {
                    var tokenString = _itoken.GenerateToken(user);
                    return new OkObjectResult(new { token = tokenString });
                }

                return new UnauthorizedObjectResult(new { message = "Unauthorized" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while logging in", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }



        public async Task<ActionResult> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                if (IsPasswordValid(newPassword))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                    if (user == null)
                    {
                        return new BadRequestObjectResult(new { message = "User not found" });
                    }

                    user.Password = HelperMethods.PasswordHelper.HashPassword(newPassword);
                    await _context.SaveChangesAsync();

                    return new OkObjectResult(new { message = "Password reset successfully" });
                }
                return new BadRequestObjectResult(new { message = "Password must be at least 8 characters long and contain at least one uppercase letter, one special character, and one numeric character." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while resetting the password", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }


        
        private Task<Users> AuthenticateUserAsync(string email, string password)
        {

            string hashedPassword = PasswordHelper.HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user == null)
            {
                throw new AuthenticationException("Invalid username or password");
            }
            return  Task.FromResult(user);
        }
        
        /*
        public int GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                if (httpContext.Items.ContainsKey("UserId") && httpContext.Items["UserId"] is int userId)
                {
                    return userId;
                }
            }

            return 0;
        }
        */
        
        public async Task<IEnumerable<UsersDto>> GetAllCandidatesAsync(int pageNumber)
        {
            try
            {
                var candidates = await _context.Users
                    .Skip((pageNumber - 1) * 10)
                    .Take(10)
                    .Select(u => new UsersDto
                    {
                        Name = u.Name,
                        Email = u.Email,
                        CreatedDate = u.CreatedDate
                    })
                    .ToListAsync();

                return candidates;
            }
            catch (Exception ex)
            {
                
                throw new ApplicationException("An error occurred while fetching candidates.", ex);
            }
        }

        public async Task<IEnumerable<UsersDto>> GetAllRecruitersAsync(int pageNumber)
        {
            try
            {
                var recruiters = await _context.Users
                    .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Recruiter"))
                     .OrderByDescending(u => u.CreatedDate)
                     .Skip((pageNumber - 1) * 10)
                     .Take(10)
                     .Select(u => new UsersDto
                     {
                         Name = u.Name,
                         Email = u.Email,
                         CreatedDate = u.CreatedDate

                     })
                     .ToListAsync();

                return recruiters;
            }
            catch (Exception ex)
            {
           
                throw new ApplicationException("An error occurred while fetching recruiters.", ex);
            }
        }

        public async Task<bool> RemoveCandidateAsync(int candidateID)
        {
            string role = await GetRoleName.GetUserRoleAsync(candidateID, _context);
            if (!role.Contains("Candidate"))
            {
                throw new InvalidOperationException("User is not a Candidate");
            }

            var user = await _context.Users
                        .Include(u => u.UserRoles)
                        .Include(u => u.JobApplications)
                        .FirstOrDefaultAsync(u => u.UserId == candidateID);
            if (user == null)
            {
                throw new InvalidOperationException("Candidate not Found");
            }
        
          
            _context.UserRoles.RemoveRange(user.UserRoles);
            _context.JobApplications.RemoveRange(user.JobApplications);
            await _context.SaveChangesAsync();

            if  (user.UserRoles.Any() || user.JobApplications.Any())
            {
                throw new InvalidOperationException("Cannot delete Recruiter with related entities.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveRecruiterAsync(int recruiterId)
        {
            string role = await GetRoleName.GetUserRoleAsync(recruiterId, _context);
            if (!role.Contains("Recruiter"))
            {
                throw new InvalidOperationException("User is not a Recruiter");
            }

            var recruiter = await _context.Users
                .Include (u => u.UserRoles)
                .Include(r => r.PostedJobs)
                .FirstOrDefaultAsync(u => u.UserId == recruiterId);               


            if (recruiter == null)
            {
                throw new ArgumentException("Recruiter not found");
            }

            _context.UserRoles.RemoveRange(recruiter.UserRoles);
            _context.Jobs.RemoveRange(recruiter.PostedJobs);
            await _context.SaveChangesAsync();

            if (recruiter.UserRoles.Any() || recruiter.PostedJobs.Any())
            {
                throw new InvalidOperationException("Cannot delete Recruiter with related entities.");
            }
            _context.Users.Remove(recruiter);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<String> ExportAllCandidatesAsync()
        {
            var candidates = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Candidate"))
                .Select(u => new UsersDto
                {
                    Name = u.Name,
                    Email = u.Email,
                    Gender = u.Gender,
                    ContactNo = u.ContactNo,
                    CreatedDate = u.CreatedDate

                })
               .ToListAsync();


            string filePath = ExcelFiles.ExportToExcel(candidates, "Candidates");
            return filePath;
        }


        public async Task<string> ExportAllRecruitersAsync()
        {
            var recruiters = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Recruiter"))
                .Select(u => new UsersDto
                { 
                    Name = u.Name,
                    Email = u.Email,
                    Gender = u.Gender,
                    ContactNo = u.ContactNo,
                    CreatedDate = u.CreatedDate

                })
               .ToListAsync();

            string filePath = ExcelFiles.ExportToExcel(recruiters, "Recruiters");
            return filePath;
        }
        
        
        public async Task<int?> GetUserIdAsync(string usernameOrEmail)
        {
            return await GetIdByName.GetUserId(_context, usernameOrEmail);
        }
        

    }


}

