using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.DTO
{
    public class RoomModel
    {
        /// <summary>
        /// Id of object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date in which the object was created
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Is object deleted ?
        /// </summary>
        public bool IsDeleted { get; set; }

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
