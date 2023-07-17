using System;
using System.Collections.Generic;
using System.Text;

namespace VodLibCore
{
    public class UserTester
    {
        public static List<User> GetMockUsers()
        {
            var users = new List<User>()
            {
                new User("TestA", "passwordA"),
                new User("TestB", "passwordB", UserRolesEnum.Admin)
            };
            return users;
        }
    }
}
