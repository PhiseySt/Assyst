using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс EventBuilder
    /// </summary>
    public class EventBuilderItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Id категории</summary>
        public long? categoryId { get; set; }
        /// <summary>Код категории категории</summary>
        public string categoryShortCode { get; set; }
        /// <summary>Название категории</summary>
        public string categoryName { get; set; }
        /// <summary>Название категории</summary>
        public string categoryVersion { get; set; }
        /// <summary>Название категории</summary>
        public string categoryDescription { get; set; }
        /// <summary>Id типа влияния</summary>
        public long seriousnessId { get; set; }
        /// <summary>Название типа влияния</summary>
        public string seriousnessName { get; set; }
        /// <summary>Id типа срочности</summary>
        public long priorityId { get; set; }
        /// <summary>Название типа срочности</summary>
        public string priorityName { get; set; }
        /// <summary>Тип события</summary>
        public long eventType { get; set; }
    }
}
