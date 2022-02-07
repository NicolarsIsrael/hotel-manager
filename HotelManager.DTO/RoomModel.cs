﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //public bool IsAvailable { get; set; }

        [Display(Name ="Next available date")]
        /// <summary>
        /// Next date and time in which the room is available for booking
        /// </summary>
        public DateTime NextAvailableDate { get; set; }

        /// <summary>
        /// Id of the current guest in the room
        /// </summary>
        public string CurrentGuest { get; set; }

        [Display(Name ="Guest full name")]
        /// <summary>
        /// Full name of the current guest occupying the room
        /// </summary>
        public string CurrentGuestFullname { get; set; }

        [Required(ErrorMessage ="Room size is required")]
        /// <summary>
        /// Size of room
        /// </summary>
        public int Size { get; set; }

    }
}
