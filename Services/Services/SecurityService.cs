using System;
using Common;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using Data.Contracts;
using Data.Repositories;
namespace Services.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public string GetSha256Hash(string input)
        {
            using var hashAlgorithm = new SHA256CryptoServiceProvider();

            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
