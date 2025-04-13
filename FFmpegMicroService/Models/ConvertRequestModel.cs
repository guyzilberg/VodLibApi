using System.Diagnostics.CodeAnalysis;

namespace FFmpegMicroService.Models
{
    public class ConvertRequestModel
    {
        #region properties
        public string UserID { get; set; }
        public string MediaID { get; set; }
        public string MediaSafeName { get; set; }

        [AllowNull]
        public int ? ConvertRequestID { get; set; }

        public string FilePath { get; set; }
        
        [AllowNull]
        public FileStatusEnum ? FileStatus { get; set; }

        [AllowNull]
        public string? ReturnUrl {  get; set; }
        #endregion

        #region c'tors
        public ConvertRequestModel(string filePath) { 
            this.FilePath = filePath;
            this.FileStatus = FileStatusEnum.Pending;
        }
        #endregion

        #region public methods
        public void UpdateStatus (FileStatusEnum fileStatus)
        {
            FileStatus = fileStatus;
        }

        public static ConvertRequestModel SetNewRequest(BackgroundServices.IConversionServiceManager _conversionServiceManager, ConvertRequestModel request)
        {
            return _conversionServiceManager.AddNewRequest(request);
        }
        #endregion

        #region enums
        public enum FileStatusEnum
        {
            None = 0,
            Pending = 1,
            Converting = 2,
            ConvertionSuccess = 3,
            ConvertionFailure = 4,
            FtpFailure = 5,
            Invalid = 6,
        }
        #endregion
    }
}
