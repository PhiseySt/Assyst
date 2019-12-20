using System.Collections.Generic;
using System.Linq;
using Assyst.Models;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        #endregion

        #region Properties & Methods

        public List<LinkedReasonItem> GetLinkedReasons()
        {
            return new List<LinkedReasonItem>()
            {
                new LinkedReasonItem {id = 1, name = "Повторное обращение"},
                new LinkedReasonItem {id = 2, name = "Изменение"},
                new LinkedReasonItem {id = 3, name = "Критический инциндент"},
                new LinkedReasonItem {id = 5, name = "Проблема"},
                new LinkedReasonItem {id = 6, name = "Задача"},
                new LinkedReasonItem {id = 7, name = "Известная ошибка"},
                new LinkedReasonItem {id = 8, name = "Затронутый объект"},
                new LinkedReasonItem {id = 9, name = "Запрос SAP"},
                new LinkedReasonItem {id = 11, name = "Основная ошибка"},
                new LinkedReasonItem {id = 12, name = "Рекламация"},
                new LinkedReasonItem {id = 13, name = "Запрос статуса"},
                new LinkedReasonItem {id = 14, name = "Коррекция данных"},
                new LinkedReasonItem {id = 15, name = "Доп. Информация"},
                new LinkedReasonItem {id = 16, name = "DINAMIC TASKS"}
            };
        }

        public List<LinkedReasonItem> GetLinkedReasons(List<long> arrayId)
        {
            return GetLinkedReasons().Where(r => arrayId.Contains(r.id)).ToList();
        }

        public string GetLinkedReasonNameById(long id)
        {
            var linkedReason = GetLinkedReasons().FirstOrDefault(r => r.id == id);
            return linkedReason?.name;
        }

        #endregion
    }
}
