using System.ComponentModel.DataAnnotations;
using VodLibCore;

namespace VodLibApi.Models.Login
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public User TryGetUser()
        {
            return User.TryGetUserByUsernameAndPassword(Username, Password);
        }
    }
}
