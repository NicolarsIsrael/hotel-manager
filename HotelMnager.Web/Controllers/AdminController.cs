using HotelManager.Core;
using HotelManager.DTO;
using HotelManager.Utility;
using HotelMnager.Web.ApiManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelMnager.Web.Controllers
{
    [Authorize(Roles =AppConstant.SuperAdminRole + "," + AppConstant.HotelStaff)]
    public class AdminController : Controller
    {
        private readonly UrlManager _urlManager;
        private readonly IRequestManager _requestManager;
        public AdminController(IRequestManager requestManager)
        {
            _urlManager = new UrlManager();
            _requestManager = requestManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validations failed");
                return View();
            }

            var result = await _requestManager.Send<RoomModel, ApiResponseModel>(_urlManager.RoomCreateNew(), model, HttpMethod.Post);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            return RedirectToAction(nameof(Rooms));
        }

        public async Task<IActionResult> Rooms()
        {
            var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.Room(), null, HttpMethod.Get);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            var rooms = JsonConvert.DeserializeObject<IEnumerable<RoomModel>>(result.data.ToString());
            return View(rooms);
        }

        [Authorize(Roles =AppConstant.SuperAdminRole)]
        public IActionResult NewHotelStaff()
        {
            return View();
        }

        [Authorize(Roles = AppConstant.SuperAdminRole)]
        [HttpPost]
        public async Task<IActionResult> NewHotelStaff(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validations failed");
                return View();
            }

            var result = await _requestManager.Send<RegisterModel, ApiResponseModel>(_urlManager.AccountCreateStaff(), model, HttpMethod.Post);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles =AppConstant.SuperAdminRole)]
        public async Task<IActionResult> Staff()
        {
            var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.AccountStaff(), null, HttpMethod.Get);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                return View();
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            var staff = JsonConvert.DeserializeObject<IEnumerable<ApplicationUser>>(result.data.ToString());
            return View(staff);
        }
    }
}
