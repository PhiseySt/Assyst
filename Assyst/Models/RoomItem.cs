using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс комнаты
    /// </summary>
    public class RoomItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Название комнаты</summary>
        public string roomName { get; set; }
        /// <summary>id здания</summary>
        public long buildingId { get; set; }
        /// <summary>Название здания</summary>
        public string buildingName { get; set; }
    }
}
