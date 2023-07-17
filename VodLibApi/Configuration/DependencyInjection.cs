using VodLibCore.Security;

namespace VodLibApi.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAwsSecrets(this IServiceCollection services, IConfigurationSection awsSecretsSection, ref AwsSecretManager awsSecretManager)
        {
            awsSecretManager = new AwsSecretManager(awsSecretsSection.GetValue<string>("Region"), awsSecretsSection.GetValue<string>("SecretName"));
            services.AddSingleton<AwsSecretManager>(awsSecretManager);
            return services;
        }
    }
}
