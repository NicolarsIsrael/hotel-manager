using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManager.DTO
{
    /// <summary>
    /// Login Model class
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Email address of user
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        /// <summary>
        /// Password of user
        /// </summary>
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    /// <summary>
    /// Register Model class for guest user
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Email address of user
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        /// <summary>
        /// Password of user
        /// </summary>
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "{0} should be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Confirm user password
        /// </summary>
        [Compare("Password", ErrorMessage = "Comfirm passsword does not match password")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "{0} is required")]
        public string FullName { get; set; }
    }
}
