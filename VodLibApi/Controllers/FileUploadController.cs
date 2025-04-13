using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Formatting;
using ThirdParty.Json.LitJson;
using VodLibApi.Models.FileUpload;
using VodLibCore;
using VodLibCore.Security;

namespace VodLibApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FileUploadController: ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<FileUploadController> _logger;
        private readonly IFileContext _fileContext;
        private readonly IMemoryCache _cache;

        public FileUploadController(IConfiguration config, ILogger<FileUploadController> logger, IFileContext fileContext, IMemoryCache cache)
        {
            _config = config;
            _logger = logger;
            _fileContext = fileContext;
            _cache = cache;
        }

        [HttpPost]
        [Route("MediaFileUpload")]
        public IActionResult MediaFileUpload([FromBody] MediaFileModel FileModel)
        {
            var userID = User.Identity.Name;
            VodLibCore.File file = FileModel.SaveNewRequest(_fileContext, _config, _logger, VodLibCore.File.FileTypes.Video, VodLibCore.File.FileStatus.Uploading);
            if(file == null)
                return BadRequest("Could not create file meta data");
            
            _cache.Set(FileModel.FileClientKey, file
                , new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(3))); // sets the cache item to be in memory for 30 seconds every time it is accessed
            return Ok(FileModel);
        }

        [HttpPost]
        [Route("UploadFileChunck")]
        public HttpResponseMessage UploadFileChunk([FromForm] IFormFile File, [FromForm] string ClientKey, [FromForm] string PartNumber, [FromForm] string TotalParts)
        {
            VodLibCore.File cachedFile = _cache.Get<VodLibCore.File>(ClientKey);
            if( cachedFile == null )
                return failedChunkUploadMessage("Invalid client Key");

            bool isComplete = false;
            using (Stream fileStream = File.OpenReadStream())
            {
                try
                {
                    isComplete = cachedFile.MergeOrCreateFileByChunks(fileStream, PartNumber, TotalParts).Result;
                }
                catch (Exception ex)
                {
                    return failedChunkUploadMessage("Failed to save File");
                }
            }
            if(isComplete == false)
                return succusfullChunkUploadMessage();

            int? uploadTaskID = cachedFile.NotifyMediaFileUloadFinished();
            return successfulFileUpload(uploadTaskID);
        }

        private HttpResponseMessage successfulFileUpload(int? uploadTaskID)
        {
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new ObjectContent<object>(new { Message = "File uploaded.", key = uploadTaskID }, new JsonMediaTypeFormatter())
            };
        }

        private HttpResponseMessage failedChunkUploadMessage(string error)
        {
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Content = new StringContent(error)
            };
        }

        private HttpResponseMessage succusfullChunkUploadMessage()
        {
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("File chunk uploaded.")
            };
        }
    }
}
