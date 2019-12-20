using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс продукта
    /// </summary>
    public class ProductItem
    {
        /// <summary>id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
        /// <summary>Id класс продукта</summary>
        public string productClassId { get; set; }
        public ProductClassItem productClass { get; set; }
    }
}
