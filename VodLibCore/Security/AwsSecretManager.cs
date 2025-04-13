using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using System;
using System.Threading.Tasks;
using Amazon;
using Newtonsoft.Json;

namespace VodLibCore.Security
{
    public class AwsSecretManager
    {
        private string _region;
        private string _secretName;
        public AwsSecretManager(string region, string secret) { 
            _region = region;
            _secretName = secret;
        }

        public async Task<string> GetSecret()
        {
            string region = _region;
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region));
            GetSecretValueRequest request = new GetSecretValueRequest();

            request.SecretId = _secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response;
            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            string secret = response.SecretString;
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(secret);
            secret = jsonObject[_secretName];
            return secret;
        }
    }
}
