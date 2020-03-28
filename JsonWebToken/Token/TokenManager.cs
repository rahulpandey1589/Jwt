using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JsonWebToken.Token
{
    public class TokenManager
    {
        private static string secretKey = ConfigurationManager.AppSettings["secretKey"];

        public static string GenerateToken(string userName)
        {
            byte[] key = Convert.FromBase64String(secretKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(GetUserClaims(userName)),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }



        private static IEnumerable<Claim> GetUserClaims(string userName)
        {
            return new List<Claim>()
           {
              new Claim(ClaimTypes.Name,userName),
              new Claim(ClaimTypes.Country,"India"),
              new Claim("Roles  ",Role.Admin.ToString()),

           };
        }
    }
}