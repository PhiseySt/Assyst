using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс назначенной группы
    /// </summary>
    public class AssignedServDeptItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Имя</summary>
        public string name { get; set; }
    }
}
