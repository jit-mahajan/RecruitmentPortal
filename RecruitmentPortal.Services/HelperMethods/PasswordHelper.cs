using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Services.HelperMethods
{
    public class PasswordHelper
    {
        [NonAction]
        public static string HashPassword(string? password)
        {
            // Generate a random salt
            byte[] salt = Encoding.UTF8.GetBytes("fixedSaltValue123");

            // Create a new instance of the Rfc2898DeriveBytes class
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            // Generate the hash value
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert the combined salt+hash to a base64-encoded string
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }
    }
}
