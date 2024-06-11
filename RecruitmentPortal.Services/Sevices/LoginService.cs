using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecruitmentPortal.Core.Entity;
using RecruitmentPortal.Infrastructure.Data;
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
    public class LoginService : ILogin
    {

        private IConfiguration _config;
        private readonly MyDbContext _context;
        private readonly List<string> _blacklistedTokens;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginService(IConfiguration configuration, MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {

            _config = configuration;
            _context = context;
            _blacklistedTokens = new List<string>();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await AuthenticateUserAsync(email, password);

                if (user != null)
                {
                    var tokenString = GenerateToken(user);
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new BadRequestObjectResult(new { message = "User not found" });
                }

                user.Password = HelperMethods.PasswordHelper.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while resetting the password", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }




        [NonAction]
        private string GenerateToken(Users users)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, users.Name),
            new Claim(JwtRegisteredClaimNames.Email, users.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };


            /*
            // Add roles as claims
            var userRoles = _context.UserRoles.Where(u => u.UserId == users.id).ToList();
            var roleIds = userRoles.Select(u => u.RoleId).ToList();
            var roles = _context.Roles.Where(r => roleIds.Contains(r.id)).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name)); // Use ClaimTypes.Role for role claims
            }

            */

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [NonAction]
        private Task<Users> AuthenticateUserAsync(string email, string password)
        {

            string hashedPassword =  HelperMethods.PasswordHelper.HashPassword(password);  
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user == null)
            {
                throw new AuthenticationException("Invalid username or password");
            }
            return Task.FromResult(user);
        }

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

        public async Task<string> GetUserIdAsync(string usernameOrEmail)
        {
            var user = await _context.Users
             .Where(u => u.Name == usernameOrEmail || u.Email == usernameOrEmail)
             .Select(u => new { u.UserId })
             .FirstOrDefaultAsync();


            return user?.UserId.ToString();
        }

    }




}
