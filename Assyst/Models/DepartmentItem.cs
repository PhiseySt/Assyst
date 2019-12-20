using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Assyst.Models
{
    /// <summary>
    /// Класс отдел
    /// </summary>
    public class DepartmentItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        /// <summary>Название</summary>
        public string name { get; set; }
        /// <summary>Id управления</summary>
        public long sectionId { get; set; }
        /// <summary>Управление</summary>
        public SectionItem section { get; set; }
    }

}
