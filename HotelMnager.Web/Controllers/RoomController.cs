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
    public class RoomController : Controller
    {
        private readonly UrlManager _urlManager;
        private readonly IRequestManager _requestManager;
        public RoomController(IRequestManager requestManager)
        {
            _urlManager = new UrlManager();
            _requestManager = requestManager;
        }

        [Authorize(Roles = AppConstant.GuestUserRole)]
        public async Task<IActionResult> Book()
        {
            try
            {
                var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.RoomGetAvailable(), null, HttpMethod.Get);
                if (!result.serverError && result.validationError)
                {
                    ModelState.AddModelError("", result.message);
                    return View();
                }
                else if (result.serverError) { return RedirectToAction("error", "Home"); }

                var rooms = JsonConvert.DeserializeObject<IEnumerable<RoomModel>>(result.data.ToString());
                return View(rooms);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = AppConstant.GuestUserRole)]
        public async Task<IActionResult> Book(int room, int days)
        {
            var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.RoomBookRoom(room, days), null, HttpMethod.Get);
            if (!result.serverError && result.validationError)
            {
                ModelState.AddModelError("", result.message);
                var _result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.RoomGetAvailable(), null, HttpMethod.Get);
                var rooms = JsonConvert.DeserializeObject<IEnumerable<RoomModel>>(_result.data.ToString());
                return View(rooms);
            }
            else if (result.serverError) { return RedirectToAction("error", "Home"); }

            return RedirectToAction(nameof(BookedSuccessfully));
        }

        [Authorize(Roles =AppConstant.GuestUserRole)]
        public async Task<IActionResult> Booked()
        {
            var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.UserBookRooms(), null, HttpMethod.Get);
            if (!result.serverError && result.validationError)
                return RedirectToAction("error", "Home");
            else if (result.serverError) { return RedirectToAction("error", "Home"); }
            var rooms = JsonConvert.DeserializeObject<IEnumerable<RoomModel>>(result.data.ToString());
            return View(rooms);
        }

        [Authorize(Roles =AppConstant.GuestUserRole)]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var result = await _requestManager.Send<string, ApiResponseModel>(_urlManager.RoomCancelBooking(id), null, HttpMethod.Get);
            if (!result.serverError && result.validationError)
                return RedirectToAction("error", "Home");
            else if (result.serverError) { return RedirectToAction("error", "Home"); }
            return RedirectToAction(nameof(CanceledBooking));
        }


        public IActionResult BookedSuccessfully()
        {
            return View();
        }

        public IActionResult CanceledBooking()
        {
            return View();
        }
    }
}
