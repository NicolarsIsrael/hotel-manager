using HotelManager.Core;
using HotelManager.DTO;
using HotelManager.Service.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Service.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenModel> GenerateToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.UserName),
                        new Claim(ClaimTypes.Name, user.FullName==null ? "" : user.FullName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
            foreach (var userRole in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public async Task<ApplicationUser> GetOrCreateExternalUserAccount(string email, string firstName, string lastName, string role)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // No user exists with this email address, we create a new one
                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        FullName = $"{firstName} {lastName}",
                    };
                    await _userManager.CreateAsync(user);
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new ApplicationRole(role));
                    await _userManager.AddToRoleAsync(user, role);
                }
                return user;
            }
            catch (Exception ex)
            {
                // log exception
                throw;
            }
        }

        public string GetLoggedInUserId()
        {
            if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
                return identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            throw new Exception(); // todo: handle exception properly
        }

        public async Task<ApplicationUser> GetLoggedInUser()
        {
            var user = await _userManager.FindByIdAsync(GetLoggedInUserId());
            if (user == null)
                throw new Exception("Unable to retrieve the logged in user details"); // todo: handle exception properly
            return user;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> CheckIfUserIsInRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> IsUserExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
