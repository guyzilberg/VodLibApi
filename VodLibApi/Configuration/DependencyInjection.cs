using VodLibCore;
using VodLibCore.Security;
using VodLibCore.Sql;

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

        public static IServiceCollection AddSqlAccess(this IServiceCollection services)
        {
            services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
            services.AddSingleton<IUserContext, UserContext>();
            services.AddSingleton<IFileContext, FileContext>();
            return services;
        }
    }
}
