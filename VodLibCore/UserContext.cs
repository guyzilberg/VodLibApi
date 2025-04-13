using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VodLibCore.Sql;

namespace VodLibCore
{
    public class UserContext : IUserContext
    {
        private readonly ISqlDataAccess _db;
        public UserContext(ISqlDataAccess db)
        {
            _db = db;
        }

        #region Crud
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _db.LoadEnumerable<User, dynamic>("dbo.SP_UserGetAll", new { });
        }

        public async Task<User> GetUserByIDAsync(int userID)
        {
            var result = await _db.LoadEnumerable<User, dynamic>("dbo.SP_UserGetByID", new { UserID = userID });
            return result.FirstOrDefault();
        }

        public async Task<User> InsertUserAsync(User user)
        {
            var parameters = user.GetContextParams(false);
            var result = await _db.SaveData<User, dynamic>(user,"dbo.SP_UserInsert", parameters);
            return result;
        }

        public async Task<User> UpdateUserByIDAsync(User user)
        {
            var result = await _db.LoadEnumerable<User, dynamic>("dbo.SP_UserUpdateByID", user.GetContextParams(true));
            return result.FirstOrDefault();
        }

        public async Task<User> DeleteUserByIDAsync(int userID)
        {
            var result = await _db.LoadEnumerable<User, dynamic>("dbo.SP_UserDeleteByID", new { UserID = userID });
            return result.FirstOrDefault();
        }

        public async Task<User> GetUserByUsername(string email)
        {
            var result = await _db.LoadEnumerable<User, dynamic>("dbo.SP_UserGetByEmail", new { Email = email });
            return result.FirstOrDefault();
        }

        #endregion
    }
}
