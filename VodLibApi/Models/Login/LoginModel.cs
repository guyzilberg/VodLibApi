using System.ComponentModel.DataAnnotations;
using VodLibCore;

namespace VodLibApi.Models.Login
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public User TryGetUser(IUserContext userContext, ILogger logger)
        {
            return User.TryGetUserByEmailAndPassword(Email, Password, userContext, logger);
        }
    }
}
