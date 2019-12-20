using System;
using System.Globalization;

namespace Assyst.Models
{
    /// <summary>
    /// Класс действие
    /// </summary>
    public class ActionItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Id типа действия</summary>
        public long actionTypeId { get; set; }
        /// <summary>Id события к которому привязано действие</summary>
        public long eventId { get; set; }
        /// <summary>Дата/время действия</summary>
        public DateTime dateActioned{ get; set; }
        /// <summary>Описание</summary>
        public string remarks { get; set; }
        /// <summary>Описание отформатированное</summary>
        public string formatedRemarks => !string.IsNullOrEmpty(remarks) ? remarks.Replace("Description From:", "Description" + Environment.NewLine + Environment.NewLine + "From:"  + Environment.NewLine).Replace(" To:", Environment.NewLine + Environment.NewLine + "To:" + Environment.NewLine) : null;
        /// <summary>Модификатор выполнения действия</summary>
        public string modifyId { get; set; }
        /// <summary>Id категории закрытия события</summary>
        public long causeCategoryId { get; set; }
        /// <summary>Id causeItem закрытия события</summary>
        public long causeItemId { get; set; }
        /// <summary>Id назначенной группы</summary>
        public long assignedServDeptId { get; set; }
        /// <summary>Id назначенного специалиста</summary>
        public long assignedUserId { get; set; }
        /// <summary>Система закрытии события</summary>
        public ObjectItem causeItem { get; set; }
        /// <summary>Категория закрытии события</summary>
        public CategoryItem causeCategory { get; set; }
        /// <summary>Тип действия</summary>
        public ActionTypeItem actionType { get; set; }
        /// <summary>Назначающая группа</summary>
        public ServiceDepartmentItem actioningServDept { get; set; }
        /// <summary>Назначающая группа</summary>
        public AssystUserItem actionedBy { get; set; }
        /// <summary>Исполняющая группа</summary>
        public ServiceDepartmentItem assignedServDept { get; set; }
        /// <summary>Исполняющая группа</summary>
        public AssystUserItem assignedUser { get; set; }
        /// <summary>Вложение к действию (для загрузки файлов)</summary>
        public AttachmentItem attach { get; set; }
        /// <summary>Вложение к действию (для получения файлов )</summary>
        public AttachmentItem[] attachments { get; set; }
        // Информация (тело события)
        public RichRemarks richRemarks { get; set; }
    }
}
