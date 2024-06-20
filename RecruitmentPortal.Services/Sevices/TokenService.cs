using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.Sevices
{
    public class TokenService : IToken
    {

        private IConfiguration _config;
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenService(IConfiguration configuration, MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(Users users)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, users.Name),
            new Claim(JwtRegisteredClaimNames.Email, users.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, users.UserId.ToString())
            };


            var userRoles = _context.UserRoles.Where(u => u.UserId == users.UserId).ToList();
            var roleIds = userRoles.Select(u => u.RoleId).ToList();
            var roles = _context.Roles.Where(r => roleIds.Contains(r.RoleId)).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
            );

            //  return new JwtSecurityTokenHandler().WriteToken(token);
            return tokenHandler.WriteToken(token);
        }

        public void InvalidateToken(string token)
        {
            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                BlacklistedAt = DateTime.UtcNow
            };
            _context.BlacklistedTokens.Add(blacklistedToken);
            _context.SaveChanges();
        }

        public bool IsTokenValid(string token)
        {
            return !_context.BlacklistedTokens.Any(t => t.Token == token);
        }
    }
}
