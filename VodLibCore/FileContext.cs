using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VodLibCore.Sql;

namespace VodLibCore
{
    public class FileContext : IFileContext
    {
        private readonly ISqlDataAccess _db;
        public FileContext(ISqlDataAccess db)
        {
            _db = db;
        }

        #region crud
        public async Task<IEnumerable<File>> GetFilesByUserID(int userID)
        {
            var result = await _db.LoadEnumerable<File, dynamic>("dbo.SP_FileGetByUserID", new { UserID = userID });
            return result;
        }

        public async Task<File> InsertFileAsync(File file)
        {
            var parameters = file.GetContextParams(false);
            var result = await _db.SaveData<File, dynamic>(file, "dbo.SP_FileInsert", parameters);
            return result;
        }

        public async Task<File> UpdateFileAsync(File file)
        {
            var result = await _db.LoadEnumerable<File, dynamic>("dbo.SP_FileUpdateByID", file.GetContextParams(true));
            return result.FirstOrDefault();
        }

        public async Task<File> DeleteFileByID(int fileID)
        {
            var result = await _db.LoadEnumerable<File, dynamic>("dbo.SP_FileDeleteByID", new { FileID = fileID });
            return result.FirstOrDefault();
        }
        #endregion
    }
}
