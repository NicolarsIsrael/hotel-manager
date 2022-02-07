using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core
{
    public class Room : Entity
    {
        /// <summary>
        /// Determines whether a room is available for booking
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Next date and time in which the room is available for booking
        /// </summary>
        public DateTime NextAvailableDate { get; set; }

        /// <summary>
        /// Id of the current guest in the room
        /// </summary>
        public string CurrentGuest { get; set; }

    }
}
