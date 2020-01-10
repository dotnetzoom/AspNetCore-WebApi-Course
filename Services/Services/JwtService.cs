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
using System.Threading;
using Data.Contracts;
using Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Services.Services
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SiteSettings _siteSetting;
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<UserTokenHandler> _repository;
        private readonly ISecurityService _securityService;

        public JwtService(IOptions<SiteSettings> settings, SignInManager<User> signInManager, IRepository<UserTokenHandler> repository, ISecurityService securityService)
        {
            _siteSetting = settings.Value;
            _signInManager = signInManager;
            _repository = repository;
            _securityService = securityService;
        }

        public async Task<AccessToken> GenerateAsync(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); // longer that 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await GetClaimsAsync(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();

            var accessTokenExpiresDateTime = DateTimeOffset.UtcNow.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes);

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");

            var expires = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;

            await AddUserTokenAsync(user, refreshToken, securityToken.ToString(), accessTokenExpiresDateTime.ToString());

            //string encryptedJwt = tokenHandler.WriteToken(securityToken);

            return new AccessToken(accessToken, refreshToken, expires);
        }

        public async Task AddUserTokenAsync(UserTokenHandler userToken)
        {
            if (!_siteSetting.JwtSettings.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);

            await _repository.AddAsync(userToken, CancellationToken.None);
        }

        public async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }
            await _repository.Table.Where(t => t.RefreshTokenIdHashSource == refreshTokenIdHashSource)
                .ForEachAsync(userToken =>
                {
                    _repository.Delete(userToken);
                });
        }

        public async Task AddUserTokenAsync(User user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial)
        {
            var now = DateTimeOffset.UtcNow;

            var token = new UserTokenHandler
            {
                UserId = user.Id,
                // Refresh token handles should be treated as secrets and should be stored hashed
                RefreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSourceSerial) ?
                    null : _securityService.GetSha256Hash(refreshTokenSourceSerial),
                AccessTokenHash = _securityService.GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = now.AddMinutes(_siteSetting.JwtSettings.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes)
            };
            await AddUserTokenAsync(token);
        }

        public async Task InvalidateUserTokensAsync(int userId)
        {
            await _repository.Table.Where(x => x.UserId == userId)
                .ForEachAsync(userToken =>
                {
                    _repository.Delete(userToken);
                });
        }

        public async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;

            await _repository.Table.Where(x => x.RefreshTokenExpiresDateTime < now)
                .ForEachAsync(userToken =>
                {
                    _repository.Delete(userToken);
                });
        }

        public Task<UserTokenHandler> FindTokenAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                return null;

            var refreshTokenSerial = GetRefreshTokenSerial(refreshTokenValue);

            if (string.IsNullOrWhiteSpace(refreshTokenSerial))
                return null;

            var refreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial);

            return _repository.TableNoTracking.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshTokenIdHash);
        }

        public string GetRefreshTokenSerial(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                return null;

            var decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                refreshTokenValue,
                new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.Issuer)),
                    ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                    ValidateLifetime = true, // validate the expiration
                    ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                },
                out _
            );

            return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);
            //add custom claims
            var list = new List<Claim>(result.Claims);
            //list.Add(new Claim(ClaimTypes.MobilePhone, "09123456987"));

            //JwtRegisteredClaimNames.Sub
            //var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;

            //var list = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //    //new Claim(ClaimTypes.MobilePhone, "09123456987"),
            //    //new Claim(securityStampClaimType, user.SecurityStamp.ToString())
            //};

            //var roles = new Role[] { new Role { Name = "Admin" } };
            //foreach (var role in roles)
            //    list.Add(new Claim(ClaimTypes.Role, role.Name));

            return list;
        }
    }
}
