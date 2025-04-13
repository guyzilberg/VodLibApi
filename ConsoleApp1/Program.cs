// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using Utils;
using VodLibCore;
using VodLibCore.Sql;


Console.WriteLine("Hello, World!");
//createKey();
createFtpDirs();
void createFtpDirs()
{
    FtpUtils ftpUtils = new FtpUtils("storage.bunnycdn.com", "guyzilmediastorage", "af23cab0-ab0e-4a15-aa9b825ef773-1160-400c", "D:\\filesToConvert\\Convert");
    ftpUtils.UploadActiveFolderAsync("D:\\filesToConvert\\Convert\\123\\1\\firstMediaUpload", new CancellationToken()).Wait();
}

void createKey()
{
    byte[] bytes = new byte[64];
    using (var provider = new RNGCryptoServiceProvider())
    {
        provider.GetBytes(bytes);
    }
    string result = BitConverter.ToString(bytes).Replace("-", "");
    Console.WriteLine(result);
    Console.ReadKey(); ;
}