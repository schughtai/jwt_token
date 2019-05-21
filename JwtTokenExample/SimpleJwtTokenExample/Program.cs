using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Program
    {
        static void Main(string[] args)
        {
            string signedAndEncodedToken = Generate("myemail@myprovider.com", "Administrator", "http://my.website.com", "http://my.sso.com");
            Console.ReadLine();

            validate(signedAndEncodedToken, "http://my.website.com", "http://my.sso.com");
            Console.ReadLine();
        }

        private static string Generate(string username, string role, string childURL, string tokenIssuerURL)
        {
            //SECRET KEY
            var plainTextSecurityKey = "This is my shared, not so secret, secret!";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.UserData, "{data:\"User serialized data can put here\"}"),
            }, "Custom");

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                AppliesToAddress = childURL,
                TokenIssuerName = tokenIssuerURL,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                Lifetime = new System.IdentityModel.Protocols.WSTrust.Lifetime(DateTime.Now, DateTime.Now.AddSeconds(2)) // EXPIRY
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            Console.WriteLine(plainToken.ToString());
            Console.WriteLine(signedAndEncodedToken);
            return signedAndEncodedToken;
        }

        private static void validate(string signedAndEncodedToken, string childURL, string tokenIssuerURL)
        {
            //SECRET
            var plainTextSecurityKey = "This is my shared, not so secret, secret!";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[] { childURL, },
                ValidIssuers = new string[] { tokenIssuerURL },
                IssuerSigningKey = signingKey
            };

            SecurityToken validatedToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            var user = tokenHandler.ValidateToken(signedAndEncodedToken,
                tokenValidationParameters, out validatedToken);


            var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var userData = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);



            Console.WriteLine(username.Value + " | " + role.Value);
            Console.WriteLine(userData.Value);
            Console.WriteLine(validatedToken.ToString());
            Console.WriteLine(validatedToken.ValidFrom);
            Console.WriteLine(validatedToken.ValidTo);
            Console.WriteLine(validatedToken.Id);
        }
    }
}
