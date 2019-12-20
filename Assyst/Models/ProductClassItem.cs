using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс класс продукта
    /// </summary>
    public class ProductClassItem
    {
        /// <summary>id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
    }
}
