using HotelManager.DTO;
using HotelMnager.Web.ApiManager;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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


        [Route("sign-up")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("sign-up")]
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

            return RedirectToAction("index", "home");
        }
    }
}
