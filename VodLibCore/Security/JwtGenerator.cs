using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using VodLibCore.Decorations;

namespace VodLibCore.Security
{
    public class JwtGenerator
    {

        #region properties
        private User user;
        private AwsSecretManager _secManager;

        private List<Claim> _claims = null;
        public List<Claim> Claims
        {
            get
            {
                if (_claims == null)
                    _claims = getUserLoginClaims(user);
                return _claims;
            }
        }
        #endregion

        #region constructors
        public JwtGenerator(User user, AwsSecretManager secretManagerService)
        {
            this.user = user;
            this._secManager = secretManagerService;
        }

        public JwtGenerator(AwsSecretManager secretManagerService)
        {
            this._secManager = secretManagerService;
        }
        #endregion

        public string GenerateJwtToken()
        {
            SymmetricSecurityKey key = GetSigninKey();
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                    issuer: getIssuer(),
                    claims: Claims,
                    expires: DateTime.UtcNow.AddDays(2), //Todo: check recommend length for tokens, for now token lasts 2 days
                    signingCredentials: creds);

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return tokenStr;
        }

        public static string GetIssuer()
        {
            return getIssuer();
        }
        private static string getIssuer()
        {
            //to do: return Issuer by application
            return "VodLib.JwtGenerator";
        }

        public static List<Claim> getUserLoginClaims(User user) // question: does it better to let the user decide what data to set in the token
        {
            List<Claim> claims = new List<Claim>();
            foreach (PropertyInfo property in user.GetType().GetProperties())
            {
                DecorationClaimable attribute = property.GetCustomAttribute<Decorations.DecorationClaimable>();
                if (attribute != null)
                {
                    Claim claim = new Claim(attribute.ClaimType, property.GetValue(user).ToString());
                    claims.Add(claim);
                }
            }

            return claims;
        }

        private static string _secretKey;
        private string getSecretKey()
        {
            if (string.IsNullOrEmpty(_secretKey))
                _secretKey = _secManager.GetSecret().Result;

            return _secretKey;
        }

        private static SymmetricSecurityKey securityKey;
        public SymmetricSecurityKey GetSigninKey()
        {
            if (securityKey == null)
                securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(getSecretKey()));

            return securityKey;
        }
        public static SymmetricSecurityKey GetSigninKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }
    }
}
