using System;
using System.Text;
using System.Security.Cryptography;

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
