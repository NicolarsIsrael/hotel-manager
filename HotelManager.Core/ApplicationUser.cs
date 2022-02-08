using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HotelManager.Core
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Full name of the user
        /// </summary>
        [Display(Name ="Full name")]
        public string FullName { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}
