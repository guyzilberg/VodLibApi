using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VodLibCore.Sql
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        public SqlDataAccess(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<IEnumerable<T>> LoadEnumerable<T, U>(string sp, U parameters)
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
            return await connection.QueryAsync<T>(sp, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<T> SaveData<T, U>(T obj ,string sp, U parameters) where T: PersistentObj
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
            int identity = await connection.ExecuteScalarAsync<int>(sp, parameters, commandType: CommandType.StoredProcedure);
            obj.OnSave<T>(identity);
            return obj;
        }
    }
}
