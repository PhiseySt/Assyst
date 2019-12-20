using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс сервисной группы
    /// </summary>
    public class ServiceDepartmentItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
    }
}
