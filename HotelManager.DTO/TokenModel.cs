using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.DTO
{
    /// <summary>
    /// Toke model class
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// token generated from user authentication
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Expriation time
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
