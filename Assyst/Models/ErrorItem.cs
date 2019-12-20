using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс ошибка
    /// </summary>
    public class ErrorItem
    {
        /// <summary>Id</summary>
        public string rule { get; set; }
        /// <summary>Код</summary>
        public string field { get; set; }
        /// <summary>Название</summary>
        public string message { get; set; }
        /// <summary>Диагностика</summary>
        public string diagnostic { get; set; }
    }
}
