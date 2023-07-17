using System;
using System.Collections.Generic;
using System.Text;
using VodLibCore.Decorations;

namespace VodLibCore
{
    public class User
    {
        [DecorationClaimable("username")]
        public string Username { get; set; }
        public string Password { get; set; }

        [DecorationClaimable("roles")]
        public UserRolesEnum Roles { get; set; }

        public User() { }
        public User(string username, string password) { 
            Username = username;
            Password = password;
            Roles = UserRolesEnum.User;
        }
        public User (string username, string password, UserRolesEnum roles) 
        { 
            Username = username;
            Password = password;
            Roles = roles;

        }

        public static User TryGetUserByUsernameAndPassword(string username, string password)
        {
            var users = UserTester.GetMockUsers();
            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                    return user;
            }

            return null;
        }
    }
}
