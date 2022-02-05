﻿using System;
using Microsoft.AspNetCore.Identity;

namespace HotelManager.Core
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}
