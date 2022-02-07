using HotelManager.Core;
using HotelManager.DTO;
using HotelManager.Service.Contract;
using HotelManager.Utility;
using Microsoft.AspNetCore.Authorization;
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
                        serverError = false,
                        validationError = false
                    });
                }
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    message = "Invalid Username or Password",
                    serverError = false,
                    validationError = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = ex.Message,
                    serverError = true
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
                    return StatusCode(StatusCodes.Status400BadRequest, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Invalid details supplied",
                        serverError = false,
                        validationError = true
                    });

                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status401Unauthorized,
                        message = "User with the email already exists",
                        serverError = false,
                        validationError = true
                    });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    FullName = model.FullName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status401Unauthorized,
                        message = "User creation failed, please check your details and try again.",
                        serverError = false,
                        validationError = true
                    });

                if (!await _roleManager.RoleExistsAsync(AppConstant.GuestUserRole))
                    await _roleManager.CreateAsync(new ApplicationRole(AppConstant.GuestUserRole));
                await _userManager.AddToRoleAsync(user, AppConstant.GuestUserRole);

                var token = await _jwtHandlerService.GenerateToken(user);
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = token,
                    serverError = false,
                    validationError = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("new-hotel-staff")]
        [HttpPost]
        [Authorize(Roles=AppConstant.SuperAdminRole)]
        public async Task<IActionResult> CreateHotelStaff([FromBody] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Invalid details supplied",
                        serverError = false,
                        validationError = true
                    });

                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status401Unauthorized,
                        message = "User with the email already exists",
                        serverError = false,
                        validationError = true
                    });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    FullName = model.FullName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status401Unauthorized,
                        message = "User creation failed, please check your details and try again.",
                        serverError = false,
                        validationError = true
                    });

                if (!await _roleManager.RoleExistsAsync(AppConstant.HotelStaff))
                    await _roleManager.CreateAsync(new ApplicationRole(AppConstant.HotelStaff));
                await _userManager.AddToRoleAsync(user, AppConstant.HotelStaff);

                var token = await _jwtHandlerService.GenerateToken(user);
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = token,
                    serverError = false,
                    validationError = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
