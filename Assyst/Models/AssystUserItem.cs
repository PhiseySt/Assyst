using System;

namespace Assyst.Models
{
    /// <summary>
    /// Класс специалиста Assyst
    /// </summary>
    public class AssystUserItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Код</summary>
        public string shortCode { get; set; }
        /// <summary>Имя</summary>
        public string name { get; set; }
        /// <summary>Id группы</summary>
        public long servDeptId { get; set; }
        public ServiceDepartmentItem servDept { get; set; }
    }
}
