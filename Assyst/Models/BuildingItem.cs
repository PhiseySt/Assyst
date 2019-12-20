using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс здания
    /// </summary>
    public class BuildingItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
        /// <summary>Город</summary>
        public string postTown { get; set; }
    }
}
