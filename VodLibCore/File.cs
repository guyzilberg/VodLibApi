using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedDtos;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VodLibCore.Decorations;
using VodLibCore.Sql;
using VodLibCore.Utils;

namespace VodLibCore
{
    public class File : PersistentObj
    {
        private readonly IFileContext _fileContext;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        #region Propeties
        [DecorationPersistent("FileID", true)]
        public int FileID { get; set; }
        [DecorationPersistent]
        public string FileExtension { get; set; }

        [DecorationPersistent("UserID")]
        public int UserID { get; set; }

        [DecorationPersistent]
        public string FilePublicTitle { get; set; }

        [DecorationPersistent]
        public string FileSafeTitle { get; set; } // this doesn't allow special chars and spaces.
        [DecorationPersistent]
        public string SavedFileName { get; set; }

        [DecorationPersistent]
        public FileTypes FileType { get; set; }

        [DecorationPersistent("FileStatus")]
        public FileStatus Status{ get; set; }

        [DecorationPersistent]
        public string FilePath
        {
            get
            {
                return getFilePath();
            }
        }
        #endregion

        #region Constructors
        public File() { }
        public File(IFileContext fileContext, ILogger logger, IConfiguration config)
        {
            _fileContext = fileContext;
            _logger = logger;
            _config = config;
        }
        public File(IFileContext fileContext, ILogger logger, IConfiguration config,  int userID, string fileExtension, string filePublicTitle, FileTypes fileType, FileStatus status) : this(fileContext, logger, config)
        {
            FileExtension = fileExtension;
            UserID = userID;
            FilePublicTitle = filePublicTitle;
            FileSafeTitle = generateSafeTitle();
            SavedFileName = Guid.NewGuid().ToString();
            FileType = fileType;
            Status = status;
        }

        public File(IFileContext fileContext, ILogger logger, IConfiguration config, int fileID, string fileExtension, int userID, string filePublicTitle, string fileSafeTitle, string savedFileName, FileTypes fileType, FileStatus status) : this(fileContext, logger, config)
        {
            FileID = fileID;
            FileExtension = fileExtension;
            UserID = userID;
            FilePublicTitle = filePublicTitle;
            FileSafeTitle = fileSafeTitle;
            SavedFileName = savedFileName;
            FileType = fileType;
            Status = status;
        }

        #endregion

        #region private methods
        private string getFilePath()
        {
            string basePath = _config.GetSection("FilesConfigs")["MediaFilesFolder"];
            return basePath + $"{UserID}\\{SavedFileName}\\{FileSafeTitle}\\";
        }
        private string generateSafeTitle()
        {
            return FilePublicTitle.RemoveSpecialChars();
        }
        #endregion
        public File Save()
        {
            try
            {
                if (FileID == 0)
                {
                    return _fileContext.InsertFileAsync(this).Result;
                }
                _fileContext.UpdateFileAsync(this);
                return this;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"failed to save file - UserID: {UserID}, FileID: {FileID}, FilePath: {FilePath}, FileName: {SavedFileName}");
                return null; 
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePart"></param>
        /// <param name="partNumber"></param>
        /// <param name="totalParts"></param>
        /// <returns>returns true if saved file completly, false if expects another chunk </returns>
        public async Task<bool> MergeOrCreateFileByChunks(Stream filePart, string partNumber, string totalParts)
        {
            try
            {
                int partNum = int.Parse(partNumber);
                int maxParts = int.Parse(totalParts);

                string filePath = getFileCompletePath();
                DirectoryUtils.EnsureDirectories(FilePath);
                using (var fileStream = new FileStream(filePath, partNum > 0 ? FileMode.Append : FileMode.Create, FileAccess.Write))
                {
                    // Copy the content of the uploaded file to the destination file stream
                    await filePart.CopyToAsync(fileStream);
                }
                return partNum == maxParts - 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string getFileCompletePath()
        {
            return FilePath + SavedFileName + FileExtension;
        }

        public int? NotifyMediaFileUloadFinished()
        {
            ConvertRequest convertRequestModel = new ConvertRequest()
            {
                UserID = this.UserID.ToString(),
                MediaID = this.FileID.ToString(),
                MediaSafeName = this.SavedFileName,
                FilePath = getFileCompletePath(),
            };
            string jsonReq = JsonConvert.SerializeObject(convertRequestModel);
            string url = _config.GetSection("FilesConfigs")["FileConvertServiceUrl"];
            try
            {
                string res = HttpUtils.PostAsync(url, jsonReq).Result;
                ConvertRequest response = JsonConvert.DeserializeObject<ConvertRequest>(res);
                return response.ConvertRequestID;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Failed to create a new conversion request, FileID: {FileID}");
                return -1;
            }
            
        }

        public enum FileTypes
        {
            None = 0,
            Text = 1,
            Image = 2,
            Video = 3,
        }

        public enum FileStatus
        {
            None = 0,
            Uploading = 0x1,
            Converting = 0x2,
            Online = 0x4,
            Private = 0x8,
            Deleted = 0x10,
        }

    }
}
