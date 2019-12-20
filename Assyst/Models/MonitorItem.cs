using System;
using System.Collections.Generic;

namespace Assyst.Models
{
    /// <summary>
    /// Класс монитора
    /// </summary>
    public class MonitorItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Назначенный отдел</summary>
        public int[] assignedServDeptId { get; set; }
        /// <summary>Статус события</summary>
        public string[] eventStatus { get; set; }
        /// <summary>Тип события</summary>
        public int[] eventType { get; set; }
        /// <summary>Таймер остановлен</summary>
        public Boolean timerStop { get; set; }
    }
}
