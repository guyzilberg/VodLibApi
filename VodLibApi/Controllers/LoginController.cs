using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        protected readonly IUserContext _userContext;
        protected readonly ILogger<LoginController> _logger;
        #endregion

        public LoginController (AwsSecretManager configAwsSecrets, IUserContext userContext, ILogger<LoginController> logger)
        {
            _awsSecretManager = configAwsSecrets;
            _userContext = userContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Index([FromBody] LoginModel loginModel)
        {
            User user = loginModel.TryGetUser(_userContext, _logger);
            if(user == null)
                return Unauthorized("Invalid Login Credentials");

            logUserConnection(user);
            JwtGenerator jwtGenerator = new JwtGenerator(user, (AwsSecretManager)_awsSecretManager);
            createCookie(jwtGenerator);
            createToken(jwtGenerator);
            return Ok(new UserModel(user));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterModel registerModel)
        {
            if (registerModel.Password != registerModel.ConfirmPassword)
                return BadRequest("passwords does not match");

            User user = _userContext.GetUserByUsername(registerModel.Username).Result;
            if (user != null)
                return BadRequest("Invalid username");

            user = registerModel.CreateNewUser(_userContext, _logger);
            if(user != null)
                return Ok();

            return BadRequest("Unable to create new user");

        }
       
        #region private methods

        private void createToken(JwtGenerator jwtGenerator)
        {
            string token = jwtGenerator.GenerateJwtToken();
            Response.Headers.Append("Authorization", "Bearer " + token);
        }

        private void createCookie(JwtGenerator jwtGenerator)
        {
            var claimsIdentity = new ClaimsIdentity(jwtGenerator.Claims, "default");
            HttpContext.SignInAsync("default", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            }).GetAwaiter().GetResult();
        }
        private void logUserConnection(User user)
        {
            //todo: make a service that tracks users ops
        }
        #endregion
    }
}
