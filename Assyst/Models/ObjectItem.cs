using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс объекта
    /// </summary>
    public class ObjectItem
    {
        /// <summary>id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
        /// <summary>Id продукта</summary>
        public long productId { get; set; }
        /// <summary>Id статуса</summary>
        public long? statusId { get; set; }
        /// <summary>Название статуса</summary>
        public string statusName { get; set; }
        /// <summary>Продукт</summary>
        public ObjectItem product { get; set; }
    }
}
