using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Assyst.Extensions;

namespace Assyst.Models
{
    /// <summary>
    /// Класс события
    /// </summary>
    public class EventItem
    {
        /// <summary>Id события</summary>
        public long id { get; set; }
        /// <summary>Заголовок</summary>
        public string title => formattedReference + " (" + eventStatusName + ") " + eventTypeName;
        /// <summary>Номер события</summary>
        public string formattedReference { get; set; }
        /// <summary>Родительская задача</summary>
        public long? parentEventId { get; set; }
        /// <summary>Id затронутого пользователя</summary>
        public long affectedUserId { get; set; }
        /// <summary>Id отдела затронутого пользователя</summary>
        public long departmentId { get; set; }
        /// <summary>Id управления отдела затронутого пользователя</summary>
        public long sectionId { get; set; }
        /// <summary>Телефон затронутого пользователя</summary>
        public string affectedUserTelephone { get; set; }
        /// <summary>Телефон добавочный затронутого пользователя</summary>
        public string affectedUserTelephoneExtension { get; set; }
        /// <summary>Описание</summary>
        public string remarks { get; set; }
        /// <summary>Дата/время последнего действия</summary>
        public DateTime lastActionDate { get; set; }
        /// <summary>Дата/время последнего изменения события</summary>
        public DateTime modifyDate { get; set; }        
        /// <summary>Объект A</summary>
        public long itemAId { get; set; }
        /// <summary>Объект B</summary>
        public long? itemBId { get; set; }  
        /// <summary>Id категории</summary>
        public long categoryId { get; set; }
        /// <summary>Id здания</summary>
        public long buildingId { get; set; }
        /// <summary>Id комнаты</summary>
        public long roomId { get; set; }
        /// <summary>Id назначенной группы</summary>
        public long assignedServDeptId { get; set; }
        /// <summary>Id назначенного пользователя</summary>
        public long assignedUserId { get; set; }
        /// <summary>Время в минутах до окончания заявки</summary>
        public string timeLeft => lastSlaClockStop == null ? EventExtensions.SetTimeLeft(olaResolveDue) : null ;
        /// <summary>Время остановки таймера</summary>
        public DateTime? lastSlaClockStop { get; set; }
        /// <summary>Дата окончания срока заявки</summary>
        public DateTime olaResolveDue { get; set; }
        /// <summary>Тип события</summary>
        public long eventType { get; set; }
        /// <summary>Название типа события</summary>
        public string eventTypeName => eventType != 0 ? ((EEventType)eventType).ToName() : null;
        /// <summary>Статус события</summary>
        public long eventStatus { get; set; }
        /// <summary>Название статуса события</summary>
        public string eventStatusName => eventStatus != 0 ? ((EEventStatus)eventStatus).ToName() : null;
        /// <summary>Подтип события</summary>
        public long eventSubType { get; set; }
        /// <summary>Id типа влияния</summary>
        public long seriousnessId { get; set; }
        /// <summary>Название типа влияния</summary>
        public string seriousnessName { get; set; }
        /// <summary>Id типа срочности</summary>
        public long priorityId { get; set; }
        /// <summary>Название типа срочности</summary>
        public string priorityName { get; set; }
        /// <summary>Режим чтения данных: 0 - с формы, 1 - с родительской заявки(создание жалобы/консультации)</summary>
        public int modeRead { get; set; }
        // Фиксация id причины для связывания с другим событием
        public int linkedReasonId { get; set; }
        // Информация
        public string webCustomPropertiesDescription { get; set; }
        // ReadOnly
        public bool readOnly { get; set; }
        // Время SLA
        public DateTime resolutionDue { get; set; }
        // Затронутый пользователь
        public ContactUserItem affectedUser { get; set; }
        // Отдел
        public DepartmentItem department { get; set; }
        // Комната
        public RoomItem room { get; set; }
        // Объект А
        public ObjectItem itemA { get; set; }
        // Объект B
        public ObjectItem itemB { get; set; }
        // Категория
        public CategoryItem category { get; set; }
        // Влияние
        public SeriousnessItem seriousness { get; set; }
        // Срочность
        public PrioritityItem priority { get; set; }
        // Назначенная группа
        public ServiceDepartmentItem assignedServDept { get; set; }
        // Назначенный специалист
        public AssystUserItem assignedUser { get; set; }
        // Родительская заявка
        public EventItem parentEvent { get; set; }
        /// <summary>Действия</summary>
        public List<ActionItem> actions { get; set; }
        // Информация (тело события)
        public RichRemarks richRemarks { get; set; }
        // Информация (тело события)
        public List<AttachmentItem> attachments { get; set; }
        /// <summary>Количество файлов вложения</summary>
        public int totalAttachmentCount { get; set; }
    }

    /// <summary>
    /// Класс для фиксации свойств событий которые меняются при редактировании
    /// </summary>
    public class FixEventItem
    {
        /// <summary>Номер события</summary>
        public long id { get; set; }
      /// <summary>Описание</summary>
        public string remarks { get; set; }
        /// <summary>Тип инциндента</summary>
        public EIncidentType incidentType { get; set; }
    }

    public enum EEventType
    {
        [Description("Инцидент")]
        INCIDENT = 1,
        [Description("Проблема")]
        PROBLEM = 2,
        [Description("Изменение, включая сервисные запросы")]
        CHANGE = 4,
        [Description("Задача")]
        NORMALTASK = 8,
        [Description("Задача принятия решений")]
        DECISIONTASK = 16,
        [Description("Задача авторизации")]
        AUTHORISATIONTASK = 32
    }

    public enum EEventStatus
    {
        [Description("Открыто")]
        OPEN = 1,
        [Description("Закрыто")]
        CLOSED = 2,
        [Description("Выполнено")]
        PENDING = 3
    }

    public enum EIncidentType
    {
        [Description("Инцидент")]
        Incident = 1,
        [Description("Консультация")]
        Consultation = 2,
        [Description("Жалоба")]
        Complaint = 3
    }

    /// <summary>
    /// Класс для фиксации свойств событий которые меняются при редактировании
    /// </summary>
    public class RichRemarks
    {
        /// <summary>Текст ckEditor</summary>
        public string content { get; set; }
        /// <summary>Простой текст</summary>
        public string plainTextContent { get; set; }
    }
}
