using System.Collections.Generic;
using System.Threading.Tasks;

namespace VodLibCore.Sql
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> LoadEnumerable<T, U>(string sp, U parameters);
        Task<T> SaveData<T, U>(T obj, string sp, U parameters) where T : PersistentObj;
    }
}