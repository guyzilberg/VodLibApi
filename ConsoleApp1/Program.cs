// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

Console.WriteLine("Hello, World!");
createKey();

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