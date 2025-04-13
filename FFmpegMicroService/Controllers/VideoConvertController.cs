using FFmpegMicroService.BackgroundServices;
using FFmpegMicroService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FFmpegMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoConvertController : ControllerBase
    {
        private readonly IConversionServiceManager _conversionServiceManager;
        private readonly ILogger<VideoConvertController> _logger;
        public VideoConvertController(IConversionServiceManager conversionServiceManager, ILogger<VideoConvertController> logger)
        {
            this._conversionServiceManager = conversionServiceManager;
            _logger = logger;   
        }

        [HttpPost]
        public IActionResult NewConvertRequest(ConvertRequestModel request)
        { 
            _logger.Log(LogLevel.Information, "New conversion request", request.ToString());
            ConvertRequestModel response = ConvertRequestModel.SetNewRequest(_conversionServiceManager, request);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllConvertRequests()
        {
            _logger.Log(LogLevel.Information, "New Get all convert request");
            return Ok(_conversionServiceManager.GetAllRequests());
        }
    }
}
