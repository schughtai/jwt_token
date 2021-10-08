using RestAPI.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using RestAPI.Auth.Model;

namespace RestAPI
{
    public class AuthHelper
    {
        //TODO: from config/ database
        private const string plainTextSecurityKey = "This is my secret key!";

        public static AuthModel GetToken(string username, string password, string companyId)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(companyId)) return null;

            AuthModel tokenObj = null;
            var apiUser = IsAuthorizedUser(username, password, companyId);
            if (apiUser == null) return new AuthModel(false, "Invalid username or password.", null);

            tokenObj = GetSecretTokenInformation(apiUser.UserId, apiUser.Roles);
            return new AuthModel(true, string.Empty, tokenObj.Token) { Expiry = tokenObj.Expiry };
        }
        public static ApiUserDto IsValidSecretKeys(string base64Secret)
        {
            var decodeauthToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Secret));

            var arrClientIdAndSecret = decodeauthToken.Split(':');
            var user = IsValidSecretKeys(arrClientIdAndSecret[0], arrClientIdAndSecret[1]);
            return user;
        }

        public static AuthModel ValidateToken(string token)
        {
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    "https://domain.com",
                    "http://127.0.01",
                },
                ValidIssuers = new string[]
                {
                    "https://domain.com",
                    "http://127.0.01",
                },
                IssuerSigningKey = signingKey
            };

            SecurityToken validatedToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            }
            catch (Exception e)
            {
                return new AuthModel() {
                    Message = e.Message,
                    IsValid = false
                };
            }
            var isValid = IsValidSecurityToken(validatedToken);

            return new AuthModel()
            {
                    Message = isValid ? "" : "Invalid token or token expired",
                    IsValid = isValid
            };
        }
        private static ApiUserDto IsAuthorizedUser(string username, string password, string companyId)
        {
            //TODO: from database
            if(username == "client")
                return new ApiUserDto() { Roles = new string[] { "client" }, UserId = "11334" };
            if(username == "customer")
                return new ApiUserDto() { Roles = new string[] { "customer" }, UserId = "14144" };
            return null;
        }

        private static AuthModel GetSecretTokenInformation(string username, string[] roles)
        {   
            // FROM CONFIG
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, string.Join(",", roles)), 
            }, "Custom");

            var expiryTime = DateTime.UtcNow.AddMinutes(720); // FROM CONFIG
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                AppliesToAddress = "https://domain.com",
                TokenIssuerName = "http://127.0.01",
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                Lifetime = new System.IdentityModel.Protocols.WSTrust.Lifetime(DateTime.UtcNow, expiryTime)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);
            return new AuthModel() { IsValid = true, Token = signedAndEncodedToken, Expiry = expiryTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") };


        }

        private static ApiUserDto IsValidSecretKeys(string username, string password)
        {
            // TODO: validate from database
            return new ApiUserDto()
            {
                UserId = "valid user id",
                Roles = null
            };
        }
        private static bool IsValidSecurityToken(SecurityToken validatedToken)
        {
            return validatedToken != null &&
                   validatedToken.ValidTo >= DateTime.UtcNow;
        }
    }
}