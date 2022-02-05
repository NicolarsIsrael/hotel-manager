using HotelManager.Core;
using HotelManager.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HotelManager.Service.Contract
{
    public interface IJwtService
    {
        /// <summary>
        /// Generate token for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<TokenModel> GenerateToken(ApplicationUser user);

        /// <summary>
        /// Get user by provider and key. Create new user if user doesn't exist
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="key"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetOrCreateExternalUserAccount(/*string provider, string key, */string email, string firstName, string lastName, string role);

        /// <summary>
        /// Get logged in user id from token
        /// </summary>
        /// <returns></returns>
        string GetLoggedInUserId();

        /// <summary>
        /// Get logged in user details
        /// </summary>
        /// <returns></returns>
        Task<ApplicationUser> GetLoggedInUser();

        /// <summary>
        /// checks if a user has a certain role
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<bool> CheckIfUserIsInRole(string email, string role);

        /// <summary>
        /// Checks if a user exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsUserExists(string email);

        /// <summary>
        /// Find user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetUserById(string userId);
    }
}
