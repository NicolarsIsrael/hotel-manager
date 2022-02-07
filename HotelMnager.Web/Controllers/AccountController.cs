using HotelManager.DTO;
using HotelManager.Utility;
using HotelMnager.Web.ApiManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelMnager.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRequestManager _requestManager;
        private readonly UrlManager _urlManager;
        public AccountController(IRequestManager requestManager)
        {
            _requestManager = requestManager;
            _urlManager = new UrlManager();
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validations failed");
                return View();
            }

            var result = await _requestManager.Send<RegisterModel, ApiResponseModel>(_urlManager.AccountSignUp(), model, HttpMethod.Post);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }else if (result.serverError) { return RedirectToAction("error", "Home"); }

            var token = JsonConvert.DeserializeObject<TokenModel>(result.data.ToString());
            StoreToken(token.Token);
            await SignInWithToken(token.Token);

            return RedirectToAction("index", "home");
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validations failed");
                return View();
            }

            var result = await _requestManager.Send<LoginModel, ApiResponseModel>(_urlManager.AccountSignIn(), model, HttpMethod.Post);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            var token = JsonConvert.DeserializeObject<TokenModel>(result.data.ToString());
            StoreToken(token.Token);
            await SignInWithToken(token.Token);
            return RedirectToAction("index", "home");
        }

        public void StoreToken(string token)
        {
            HttpContext.Session.SetString(AppConstant.AccessToken, token);
        }

        public async Task SignInWithToken(string token, bool isPersistent = true)
        {
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetString(AppConstant.AccessToken, "");
            await SignOutUser();
            return RedirectToAction("index", "Home");
        }

        public async Task SignOutUser()
        {
            HttpContext.Session.SetString(AppConstant.AccessToken, "");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
