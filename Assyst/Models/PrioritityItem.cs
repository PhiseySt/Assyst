using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс срочность
    /// </summary>
    public class PrioritityItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
    }
}
