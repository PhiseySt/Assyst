using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс категории
    /// </summary>
    public class CategoryItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }

    }
}
