using HotelManager.Core;
using HotelManager.DTO;
using HotelManager.Service.Contract;
using HotelManager.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly IJwtService _jwtHandlerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
        {
            _jwtHandlerService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = await _jwtHandlerService.GenerateToken(user);
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status200OK,
                        data = token,
                        hasError = false
                    });
                }
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid Username or Password",
                    hasError = true
                });
            }
            catch (Exception ex)
            {
                // log error ;_logger.Log(LogLevel.Error, ex, "SignIn");
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = ex.Message,
                    hasError = true
                });
            }
        }

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid details supplied");

                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "User already exists");
                // todo: handle user already exists properly

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    FullName = model.FullName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, "User creation failed! Please check user details and try again.");
                // todo: handle exception properly

                if (!await _roleManager.RoleExistsAsync(AppConstant.GuestUserRole))
                    await _roleManager.CreateAsync(new ApplicationRole(AppConstant.GuestUserRole));
                await _userManager.AddToRoleAsync(user, AppConstant.GuestUserRole);

                var token = await _jwtHandlerService.GenerateToken(user);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
