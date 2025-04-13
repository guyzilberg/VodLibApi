using VodLibCore;

namespace VodLibApi.Models.Login
{
    [Serializable]
    public class UserModel
    {
        public string Email { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public UserRoles Roles { get; set; }

        public UserModel(User user)
        {
            Email = user.Email;
            UserID = user.UserID;
            Username = user.Username;
            Roles = user.Roles;

        }
    }
}
