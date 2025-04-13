using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class FtpUtils
    {
        private string ftpServer;
        private string ftpUserName;
        private string ftpPassword;
        private string ignorePrefix;
        public FtpUtils(string server, string username, string password, string ignorePathPrefix) 
        { 
            this.ftpServer = server;
            this.ftpUserName = username;
            this.ftpPassword = password;
            this.ignorePrefix = ignorePathPrefix;
        }
        /// <summary>
        /// This method will continuesly check for new files in the folder, and will upload them to ftpServer, untill it receives a cancelation token 
        /// or until 2 minutes has passed and no new file was generated on the folder
        /// </summary>
        public async Task UploadActiveFolderAsync(string folderPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(folderPath)
                || Directory.Exists(folderPath) == false)
                return;

            string ftpFolder = createPathInFtpServer(folderPath, ignorePrefix);

            List<string> filesUploaded = new List<string>();
            bool timeout = false;
            while (cancellationToken.IsCancellationRequested == false && timeout == false) 
            {
                bool hasNewFiles;
                List<string> filesToUpload = getFilesToUpload(folderPath, filesUploaded, out hasNewFiles);
                if(hasNewFiles == false) 
                    timeout = checkForTimeout();
                
                else
                    foreach (string filepath in filesToUpload) 
                    {
                        string filename = Path.GetFileName(filepath);
                        UploadFile(ftpFolder, filepath, filename);
                        filesUploaded.Add(filepath);
                    } 
            }
        }

        private string createPathInFtpServer(string folderPath, string ignorePrefix)
        {
            string[] dirs = folderPath.Replace(ignorePrefix, "").Split(Path.DirectorySeparatorChar);
            string basePath = "ftp://" + this.ftpServer;
            for (int i=1; i<dirs.Length; i++) // do not use the disk partition as path (e.g D:)
            {
                try
                {
                    basePath += $"/{dirs[i]}";
                    FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create(basePath);
                    ftpClient.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
                    ftpClient.Method = WebRequestMethods.Ftp.MakeDirectory;

                    FtpWebResponse response = (FtpWebResponse)ftpClient.GetResponse();
                    response.Close();   
                }
                catch (Exception ex)
                { 
                    //to do: log
                }
            }
            return basePath;
        }

        private List<string> getFilesToUpload(string folderPath, List<string> filesUploaded, out bool hasNewFiles)
        {
            List<string> filesInFolder= Directory.GetFiles(folderPath).ToList();
            List<string> newFiles = filesInFolder.Except(filesUploaded).ToList();
            if(newFiles != null && newFiles.Count > 0)
                hasNewFiles = true;
            else
                hasNewFiles= false;

            return newFiles;
        }

        private bool checkForTimeout()
        {
            //to do: make a making that sets timeout after 2 minutes
            return false;
        }

        public void UploadFile(string ftpFolder,string filepath, string filename)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(this.ftpUserName, ftpPassword);

                    // Specify the FTP server and the destination file path.
                    string remoteFilePath = ftpFolder + "/" + filename;

                    // Upload the file.
                    client.UploadFile(remoteFilePath, filepath);
                }
            }
            catch (WebException e)
            {
                //to do: create a logger
            }
        }
    }
}
