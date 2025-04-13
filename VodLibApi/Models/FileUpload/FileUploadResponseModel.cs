using VodLibCore;

namespace VodLibApi.Models.FileUpload
{
    public class FileUploadResponseModel
    {
        #region properties
        public UplaodStatus ResultCode { get; set; }
        public string Result { get; set; }
        #endregion

        #region constructors
        public FileUploadResponseModel()
        {
        }

        public FileUploadResponseModel(UplaodStatus resultCode, string result)
        {
            ResultCode = resultCode;
            Result = result;
        }

        public static string GenerateFilePath(string folderPath, string fileExtension, int userID)
        {
            return FilesFacade.GenerateFilePath(folderPath, fileExtension, userID);
        }

        #endregion

        public enum UplaodStatus
        {
            Success = 0,
            Nofile = -1,
            SaveFailed = -2
        }

    }
}
