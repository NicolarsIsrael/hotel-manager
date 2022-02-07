using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManager.Core
{
    /// <summary>
    /// Represents the basic properties of all tables
    /// </summary>
    public class Entity
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
    }
}
