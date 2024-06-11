using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentPortal.Core.Entity;
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
     public class RegisterService :IRegistration
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
        public async Task<ActionResult> RegisterAsync(Users users)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == users.Email);
                if (existingUser != null)
                {
                    return new ConflictObjectResult(new { message = "Username already exists" });
                }
           
             
                if (!IsPasswordValid(users.Password))
                {
                    return new BadRequestObjectResult(new { message = "Password must be at least 8 characters long and contain at least one uppercase letter, one special character, and one numeric character." });
                }

                string hashedPassword = HelperMethods.PasswordHelper.HashPassword(users.Password);     //.HashPassword(users.Password);


                Users user = new Users
                {
                    Name = users.Name,
                    Gender = users.Gender,
                    ContactNo = users.ContactNo,
                    Email = users.Email,
                    Password = hashedPassword,
                    IsActive = true
                };
                
                await _context.AddAsync(user);
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

        [NonAction]
        public bool IsPasswordValid(string? password)
        {

            if (password == null)
            {
                return false;
            }
            // Define regular expression pattern for password complexity
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$";

            // Check if password matches the pattern
            return Regex.IsMatch(password, pattern);
        }


    }
}
