using System.Collections.Generic;

namespace Assyst.Models
{
    /// <summary>
    /// Класс настройки Popup
    /// </summary>
    public class PopupSettings
    {
        /// <summary>Название</summary>
        public string name { get; set; }
        /// <summary>Url контента</summary>
        public string contentUrl { get; set; }
        /// <summary>Ширина</summary>
        public int width { get; set; }
        /// <summary>Высота</summary>
        public int height { get; set; }
        /// <summary>Заголовок окна</summary>
        public string headerText { get; set; }
        /// <summary>Не редактировать</summary>
        public bool showFooter { get; set; }
        /// <summary>Событие закрытия</summary>
        public string closingEvent { get; set; }
        /// <summary>Событие нажатия на кнопку Сохранить</summary>
        public string submitButtonClickEvent { get; set; }
        /// <summary>Событие нажатия на кнопку Отменить</summary>
        public string closeButtonClickEvent { get; set; }
    }
}
