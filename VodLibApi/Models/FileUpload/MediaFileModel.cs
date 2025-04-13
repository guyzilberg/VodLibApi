using System.Diagnostics.CodeAnalysis;
using static VodLibCore.File;

namespace VodLibApi.Models.FileUpload
{
    public class MediaFileModel
    {
        public int UserID { get; set; }
        public string FileTitle { get; set; }
        public string FileDescription { get; set; }
        public string? FileExtension { get; set; }
        
        [AllowNull]
        public string? FileClientKey { get; set; }

        public VodLibCore.File SaveNewRequest(VodLibCore.IFileContext fileContext, IConfiguration config, ILogger<Controllers.FileUploadController> logger, FileTypes fileType,FileStatus fileStatus)
        {
            if (string.IsNullOrWhiteSpace(FileClientKey))
                FileClientKey = Guid.NewGuid().ToString();

            VodLibCore.File file = new VodLibCore.File(fileContext, logger, config, UserID, FileExtension, FileTitle, fileType, fileStatus);
            file = file.Save();
            return file;
        }
    }
}
