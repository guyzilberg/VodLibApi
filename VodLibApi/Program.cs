using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using VodLibCore.Security;
using VodLibApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AwsSecretManager awsSecretManager = null;
builder.Services.AddAwsSecrets(builder.Configuration.GetSection("AwsSecrets"), ref awsSecretManager);
builder.Services.AddMemoryCache();
builder.Services.AddSqlAccess();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                                .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                                .WithExposedHeaders("Authorization");
        });
});

addAuthentication(builder, awsSecretManager);
builder.Services.AddAuthorization(
    options =>
    {
        var defAuthPolicy = new AuthorizationPolicyBuilder(
            JwtBearerDefaults.AuthenticationScheme,
            "default-vodlib"
            );
        options.DefaultPolicy = defAuthPolicy.RequireAuthenticatedUser().Build();
    });
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

app.UseRouting();
if (app.Environment.IsDevelopment())
    app.UseCors("DevelopmentPolicy");

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    var cookie = ctx.Request.Headers.Cookie;
    await next(ctx);
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void addAuthentication(WebApplicationBuilder builder, AwsSecretManager awsSecretManager)
{
    const string COOKIE_NAME = "default-vodlib";
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = COOKIE_NAME;
        options.DefaultChallengeScheme = COOKIE_NAME;
        
    }).AddCookie("default", options =>
    {
        options.Cookie.Name = COOKIE_NAME;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    }).AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
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