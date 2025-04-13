using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VodLibCore.Utils
{
    public class HttpUtils
    {
        public static async Task<string> PostAsync(string url, string jsonData, List<KeyValuePair<string, string>> headersList = null)
        {
            using (var client = new HttpClient())
            {
                // Create the HttpContent with JSON data
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Add headers if provided
                if (headersList != null)
                {
                    foreach (var header in headersList)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                // Send the POST request asynchronously
                var response = await client.PostAsync(url, content);

                // Check for successful response
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as string
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

    }
}
