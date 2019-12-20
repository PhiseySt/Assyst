using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс управления
    /// </summary>
    public class SectionItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
    }
}
