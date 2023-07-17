using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VodLibApi.Models.Login;
using VodLibCore;
using VodLibCore.Security;

namespace VodLibApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        #region dependencies
        protected readonly AwsSecretManager _awsSecretManager;
        #endregion

        public LoginController (AwsSecretManager configAwsSecrets)
        {
            _awsSecretManager = configAwsSecrets;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index([FromBody] LoginModel loginModel)
        {
            User user = tryGetUserByUserameAndpass(loginModel);
            if(user == null)
                return Unauthorized("Invalid Login Credentials");

            logUserConnection(user);
            JwtGenerator jwtGenerator = new JwtGenerator(user, (AwsSecretManager)_awsSecretManager);
            createCookie(jwtGenerator);
            createToken(jwtGenerator);
            return Ok();
        }

        private void createToken(JwtGenerator jwtGenerator)
        {
            string token = jwtGenerator.GenerateJwtToken();
            Response.Headers.Append("Authorization", "Bearer " + token);
        }

        private void createCookie(JwtGenerator jwtGenerator)
        {
            var claimsIdentity = new ClaimsIdentity(jwtGenerator.Claims, "default");
            HttpContext.SignInAsync("default", new ClaimsPrincipal(claimsIdentity)).GetAwaiter().GetResult();
        }

        #region private methods

        private void logUserConnection(User user)
        {
            //todo: make a service that tracks users ops
        }

        private User tryGetUserByUserameAndpass(LoginModel loginModel)
        {
            return loginModel.TryGetUser();
        }
        #endregion
    }
}
