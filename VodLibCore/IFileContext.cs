using System.Collections.Generic;
using System.Threading.Tasks;

namespace VodLibCore
{
    public interface IFileContext
    {
        Task<File> DeleteFileByID(int fileID);
        Task<IEnumerable<File>> GetFilesByUserID(int userID);
        Task<File> InsertFileAsync(File file);
        Task<File> UpdateFileAsync(File file);
    }
}