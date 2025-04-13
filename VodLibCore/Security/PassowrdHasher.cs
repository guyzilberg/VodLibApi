using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace VodLibCore.Security
{
    public class PassowrdHasher
    {
        private const int WorkFactor = 12; // The work factor determines the number of iterations (2^workFactor) for the hashing algorithm.

        public static string GenerateRandomSalt()
        {
            byte[] saltBytes = new byte[16]; // 16 bytes is a recommended size for a salt.
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            string saltedPassword = password + salt;
            return BCrypt.Net.BCrypt.HashPassword(saltedPassword, WorkFactor);
        }

        public static bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            string saltedPassword = password + salt;
            return BCrypt.Net.BCrypt.Verify(saltedPassword, hashedPassword);
        }
    }
}
