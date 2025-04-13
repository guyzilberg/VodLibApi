using FFmpegMicroService.Models;
using System.Text;
using System;
using System.Text.Json;

namespace FFmpegMicroService.BackgroundServices
{
    public interface IConversionServiceManager
    {
        public ConvertRequestModel AddNewRequest(ConvertRequestModel request);
        public List<ConvertRequestModel> GetAllRequests();
        public ConvertRequestModel GetPendingNextFile();
        public void Save();
        void UpdateStatus(ConvertRequestModel conversionFile, ConvertRequestModel.FileStatusEnum convertionSuccess);
    }

    public class ConversionServiceManager : IConversionServiceManager
    {
        private const string FilePath = "ConversionFile.txt";
        private List<ConvertRequestModel> pendingFileModels;

        public ConversionServiceManager()
        {
              try
              {
                  string json = File.ReadAllText(FilePath);
                  if (string.IsNullOrEmpty(json) == false)
                      pendingFileModels = JsonSerializer.Deserialize<List<ConvertRequestModel>>(json);
                  else
                    pendingFileModels = new List<ConvertRequestModel>();
              }
              catch (Exception ex)
              {
                  pendingFileModels = new List<ConvertRequestModel>();
              }
        }

        public ConvertRequestModel AddNewRequest(ConvertRequestModel request)
        {
            if (verifyUser(request.UserID) == false)
                return requestInvalid(request);

            request.ConvertRequestID = pendingFileModels.Count;
            request.UpdateStatus(ConvertRequestModel.FileStatusEnum.Pending);
            pendingFileModels.Add(request);

            Save();
            return request;
        }

        private bool verifyUser(string userID)
        {
            //to do : make a call to main api that checks whether user is allowed to upload files or that file is convertable
            return true;
        }

        private ConvertRequestModel requestInvalid(ConvertRequestModel request)
        {
            request.UpdateStatus(ConvertRequestModel.FileStatusEnum.Invalid);
            request.ConvertRequestID = -1;

            return request;
        }

        public List<ConvertRequestModel> GetAllRequests()
        {
            return pendingFileModels;
        }

        public ConvertRequestModel GetPendingNextFile()
        {
            if (pendingFileModels.Count == 0)
                return null;

            var file = pendingFileModels.FirstOrDefault(x => x.FileStatus == ConvertRequestModel.FileStatusEnum.Pending);
            if (file == null)
                return null;

            file.UpdateStatus(ConvertRequestModel.FileStatusEnum.Converting);
            Save();
            return file;
        }

        public void Save()
        {
            saveToFile();
        }

        private void saveToFile()
        {
            string json = JsonSerializer.Serialize(pendingFileModels); //checking out the improved .net 8 json converstion library
            File.WriteAllText(FilePath, json);
        }

        public void UpdateStatus(ConvertRequestModel conversionFile, ConvertRequestModel.FileStatusEnum status)
        {
            conversionFile.UpdateStatus(status);
            if ((status == ConvertRequestModel.FileStatusEnum.ConvertionSuccess
               || status == ConvertRequestModel.FileStatusEnum.ConvertionFailure
               )
               && string.IsNullOrEmpty(conversionFile.ReturnUrl) == false
               )
                notifyConverssionEnd(conversionFile);
                
            Save();
        }

        private void notifyConverssionEnd(ConvertRequestModel conversionFile)
        {
            string jsonData = JsonSerializer.Serialize(conversionFile);
            using (var client = new HttpClient())
            {
                // Create the HttpContent with JSON data
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


               // Send the POST request asynchronously
                var response = client.PostAsync(conversionFile.ReturnUrl, content).Result;

                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as string
                      string result = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    
                }
            }
        }
    }
}
