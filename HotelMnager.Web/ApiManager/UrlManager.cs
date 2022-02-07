using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMnager.Web.ApiManager
{
    public class UrlManager
    {
        public string ApiBase { get; set; }
        public UrlManager()
        {
            ApiBase = "/api/";
        }

        #region account controller
        public string AccountSignUp() { return $"{ApiBase}account/sign-up"; }
        public string AccountSignIn() { return $"{ApiBase}account/sign-in"; }
        #endregion

        #region room controller
        public string Room() { return $"{ApiBase}room"; }
        public string RoomGetAvailable() { return $"{ApiBase}room/available"; }
        public string RoomCreateNew() { return $"{ApiBase}room/create-new-room"; }
        public string RoomBookRoom(int id, int days) { return $"{ApiBase}room/book/{id}?days={days}"; }
        public string RoomCancelBooking(int id, int days) { return $"{ApiBase}room/cancel-booking/{id}?days={days}"; }
        #endregion
    }
}
