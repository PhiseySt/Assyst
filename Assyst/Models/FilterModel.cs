using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assyst.Models
{
    public class FilterModel
    {
        /// <summary>Имя пользователя системы</summary>
        public string userName { get; set; }
        /// <summary>Фильтр "таймер остановлен"</summary>
        public bool fltrTimerStop { get; set; }
        /// <summary>Фильтр "назначенные на меня"</summary>
        public bool fltrMyEvents { get; set; }
        /// <summary>Массив фильтров "типов событий"</summary>
        public long[] fltrEventType { get; set; }
    }
}
