using System.Collections.Generic;
using System.Threading.Tasks;

namespace VodLibCore
{
    public interface IUserContext
    {
        Task<User> DeleteUserByIDAsync(int UserID);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIDAsync(int UserID);
        Task<User> GetUserByUsername(string username);
        Task<User> InsertUserAsync(User user);
        Task<User> UpdateUserByIDAsync(User user);
    }
}