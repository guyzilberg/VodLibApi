using FFmpegMicroService.Models;
using System.Diagnostics;
using Utils; 
namespace FFmpegMicroService.BackgroundServices
{
    public class ConversionService : BackgroundService
    {
        private static bool isConverting;
        private string ffmpegPath;//to do: get from configuration
        private string outputBaseFolderPath;
        private readonly IConfiguration _config;
        private readonly IConversionServiceManager _serviceManager;

         public ConversionService(IConfiguration configuration, IConversionServiceManager serviceManager)
         {
             _config = configuration;
            var configSection = _config.GetSection("BackgroundService");
            ffmpegPath = configSection.GetValue<string>("FfmpegPath");
            outputBaseFolderPath = configSection.GetValue<string>("OutputBasePath");
            _serviceManager = serviceManager;
        } 
             
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            while (stoppingToken.IsCancellationRequested == false)
            {
                if(isConverting == false)
                {
                    isConverting = true;

                    var conversionFile = getNextFileToConvert();
                    if (conversionFile != null)
                    {
                        if (string.IsNullOrEmpty(conversionFile.FilePath) == false && File.Exists(conversionFile.FilePath))
                        {
                            string outputFolderPath = getOutputFolderPath(outputBaseFolderPath, conversionFile);
                            FilesUtils.EnsureDirectories(outputFolderPath);
                            ConvertRequestModel.FileStatusEnum status =await startNewConverstion(conversionFile.FilePath, outputFolderPath);
                            _serviceManager.UpdateStatus(conversionFile, status);
                            //conversionFile.UpdateStatus(ConvertRequestModel.FileStatusEnum.ConvertionSuccess);
                        }
                    }
                    isConverting = false;
                }
                await Task.Delay(1000 * 60 * 1, stoppingToken);
            }   
        }

        private string getOutputFolderPath(string baseDirectory, ConvertRequestModel conversionFile)
        {
            return baseDirectory + conversionFile.UserID 
                + Path.DirectorySeparatorChar + conversionFile.MediaID
                + Path.DirectorySeparatorChar + conversionFile.MediaSafeName;
        }

        private async Task<ConvertRequestModel.FileStatusEnum> startNewConverstion(string filePath, string outputFolderPath)
        {
            string ffmpegArgs = $" -i {filePath} -c:v libx264 -c:a aac -hls_time 2 -hls_list_size 0 -hls_segment_filename {outputFolderPath}\\output_%03d.ts {outputFolderPath}\\output.m3u8";
            return await startFFmpegProcess(ffmpegArgs, outputFolderPath);
        }

        private async Task<ConvertRequestModel.FileStatusEnum> startFFmpegProcess(string ffmpegArgs, string outputFolderPath) 
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            try
            {
                // Define the process start information
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath, // Replace with the path to the executable you want to run
                                              // You can also specify arguments if needed, like:
                                              // Arguments = "myfile.txt",
                    UseShellExecute = true, // Set to true if you want to use the system's shell to start the process
                    Arguments = ffmpegArgs
                };

                // Start the process
                Process process = Process.Start(startInfo);

                Task ftpUpload = startFtpUpload(outputFolderPath, token);
                // Optionally, wait for the process to exit
                process.WaitForExit();

                tokenSource.Cancel();// to do: this might stop too early, need to figure out if that is the case, what are the possibilities one option is to pass another token that notifies that conversion has finished and make one last run over files in folder. 
                await ftpUpload;

                return ConvertRequestModel.FileStatusEnum.ConvertionSuccess;
                //to do: logToLogger
//                Console.WriteLine("Process exited with code: " + process.ExitCode);
            }
            catch (Exception e)
            {
                tokenSource.Cancel();
                return ConvertRequestModel.FileStatusEnum.ConvertionFailure;
   //             Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        private Task startFtpUpload(string outputFolderPath, CancellationToken token)
        {
            var ftpData = _config.GetSection("BnetFtp");
            
            FtpUtils ftp = new FtpUtils(ftpData.GetValue<string>("Server"), ftpData.GetValue<string>("Username"), ftpData.GetValue<string>("Password"), outputBaseFolderPath);
            return ftp.UploadActiveFolderAsync(outputFolderPath, token);
        }

        private ConvertRequestModel getNextFileToConvert()
        {
            return _serviceManager.GetPendingNextFile();
        }
    }
}
