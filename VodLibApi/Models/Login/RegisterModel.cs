using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using VodLibCore;
using VodLibCore.Security;

namespace VodLibApi.Models.Login
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; } 
        public string Password { get; set; }
        public string Email { get; set; }
        public string ConfirmPassword { get; set; }

        public User CreateNewUser(IUserContext userContext, ILogger logger)
        {
            string salt = VodLibCore.Security.PassowrdHasher.GenerateRandomSalt();
            string hashedPassword = VodLibCore.Security.PassowrdHasher.HashPassword(Password, salt);
            User user = new User(Username, Email, hashedPassword, salt, userContext, logger);
            user = user.Save();
            return user;
        }
    }
}
