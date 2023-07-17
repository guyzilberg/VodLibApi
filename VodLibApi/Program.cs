using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using VodLibCore.Security;
using Microsoft.Extensions.Configuration;
using VodLibApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AwsSecretManager awsSecretManager = null;
builder.Services.AddAwsSecrets(builder.Configuration.GetSection("AwsSecrets"), ref awsSecretManager);
addAuthentication(builder, awsSecretManager);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void addAuthentication(WebApplicationBuilder builder, AwsSecretManager awsSecretManager)
{
    const string COOKIE_NAME = "COOKIE_OR_JWT";
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = COOKIE_NAME;
        options.DefaultChallengeScheme = COOKIE_NAME;
    }).AddCookie("default", options =>
    {
        options.Cookie.Name = "default-vodlib";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);/* browser will delete cookie after one day
                                                        by default .net tells the browser to keep it until window is closed
                                                       */
    }).
    AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = JwtGenerator.GetIssuer(),
            IssuerSigningKey = JwtGenerator.GetSigninKey(awsSecretManager.GetSecret().Result)
        };
    }
    ).
    AddPolicyScheme(COOKIE_NAME, COOKIE_NAME, options => {
        options.ForwardDefaultSelector = context =>
        {
            string bearerToken = context.Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(bearerToken) == false && isBearerToken(bearerToken))
                return "Bearer";

            return "default";
        };
    });

}

bool isBearerToken(string bearerToken)
{
    return bearerToken.StartsWith("Bearer ");
}