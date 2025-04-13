using System;
using VodLibCore.Decorations;
using VodLibCore.Security;
using VodLibCore.Sql;
using Microsoft.Extensions.Logging;
namespace VodLibCore
{
    public class User : PersistentObj
    {
        private readonly IUserContext _userContext;
        private readonly ILogger _logger;
        [DecorationPersistent("UserID", true)]
        [DecorationClaimable("UserID")]
        public int UserID { get; set; }

        [DecorationPersistent]
        [DecorationClaimable("Username")]
        public string Username { get; set; }

        [DecorationPersistent]
        public string Password { get; set; }

        [DecorationPersistent("PasswordSalt")]
        public string PasswordSalt { get; set; }

        [DecorationPersistent("UserRole")]
        [DecorationClaimable("roles")]
        public UserRoles Roles { get; set; }

        [DecorationPersistent]
        public string Email { get; set; }

        public User()
        {

        }
        public User(IUserContext userContext, ILogger logger) 
        {
            this._userContext = userContext;
            this._logger = logger;
        }
        public User(string username, string email, string password, string salt, IUserContext userContext, ILogger logger)
        {
            Username = username;
            Password = password;
            Email = email;
            PasswordSalt = salt;
            Roles = UserRoles.User;
            this._userContext = userContext;
            this._logger = logger;
        }
        public User(string username, string email, string password, string salt, UserRoles roles, IUserContext userContext, ILogger logger)
        {
            Username = username;
            Password = password;
            PasswordSalt = salt;
            Email = email;
            Roles = roles;
            this._userContext = userContext;
            this._logger = logger;
        }

        public static User TryGetUserByEmailAndPassword(string email, string password, IUserContext userContext, ILogger logger)
        {
            var user = userContext.GetUserByUsername(email).Result;

            if (user == null)
                return null;

            if (PassowrdHasher.VerifyPassword(password, user.PasswordSalt, user.Password))
                return user;

            return null;
        }

        public User Save()
        {
            try
            {
                if (UserID == 0)// creates new User
                    return _userContext.InsertUserAsync(this).Result;

                _userContext.UpdateUserByIDAsync(this);
                return this;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Error while saving user - UserID: {0}, Message: {1}", UserID, ex.Message);
                return null;
            }
        }

    }
}
