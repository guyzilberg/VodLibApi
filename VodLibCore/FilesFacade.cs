using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VodLibCore
{
    public class FilesFacade
    {
        public static void CompleteFileUpload(string filePath, int userID, string fileTitle, string fileDescription, FileType media)
        {
            //to do: 1. save file to DB
            //       2. convert to hls if media
            //       3. upload to bunny net if media
        }

        public static string GenerateFilePath(string folderPath, string fileExtension, int userID)
        {
            if (Directory.Exists(folderPath) == false)
                Directory.CreateDirectory(folderPath);


            if (Directory.Exists(folderPath + "\\" + userID) == false)
                Directory.CreateDirectory(folderPath + "\\" + userID);

            string fileName = Guid.NewGuid().ToString("N") + ensureDot(fileExtension) + fileExtension;
            return fileName;
        }

        private static string ensureDot(string fileExtension)
        {
            if (fileExtension.StartsWith("."))
                return string.Empty;

            return ".";
        }
        public enum FileType
        {
            Media = 1
        }
    }
}
