using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core
{
    public class Room : Entity
    {
        /// <summary>
        /// Next date and time in which the room is available for booking
        /// </summary>
        public DateTime NextAvailableDate { get; set; }

        /// <summary>
        /// Id of the current guest in the room
        /// </summary>
        public string CurrentGuest { get; set; }

        /// <summary>
        /// Size of room e.g 2-man room
        /// </summary>
        public int Size { get; set; }

    }
}
