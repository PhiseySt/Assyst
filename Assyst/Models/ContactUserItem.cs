using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс пользователей
    /// </summary>
    public class ContactUserItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Имя</summary>
        public string name { get; set; }
        /// <summary>Id отдела</summary>
        public long departmentId { get; set; }
        /// <summary>Id комнаты</summary>
        public long roomId { get; set; }
        /// <summary>Отдел</summary>
        public DepartmentItem department { get; set; }
        /// <summary>Комната</summary>
        public RoomItem room { get; set; }
        /// <summary>Телефон</summary>
        public string officeTelephone { get; set; }
        /// <summary>Добавочный телефон</summary>
        public string officeTelephoneExtension { get; set; }
        /// <summary>Email</summary>
        public string emailAddress { get; set; }

    }
}
