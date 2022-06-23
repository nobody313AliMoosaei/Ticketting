using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api_Center.Models
{
    public class MethodHelper : IMethodHelper
    {
        private readonly IConfiguration Configuration;
        public MethodHelper(IConfiguration con)
        {
            Configuration = con;
        }

        public  string CreatToken(int UserId, string UserName)
        {
            if (UserId == 0 || string.IsNullOrEmpty(UserName))
                return string.Empty;
            List<Claim> Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,UserId.ToString()),
                new Claim(ClaimTypes.Name,UserName)
            };
            string Key = Configuration["JWTConfiguration:key"];
            var SecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var crendtials = new SigningCredentials(SecretKey, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer: Configuration["JWTConfiguration:issuer"],
                audience: Configuration["JWTConfiguration:audience"],
                expires: DateTime.Now.AddDays(10),
                notBefore: DateTime.Now,
                claims: Claims,
                signingCredentials: crendtials);

            var JWTToken = new JwtSecurityTokenHandler().WriteToken(Token);

            return JWTToken;
        }
        public string CreatRefreshToken()
        {
            return
                Guid.NewGuid().ToString();
        }
    }
}
