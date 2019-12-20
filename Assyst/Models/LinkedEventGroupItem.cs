using System;
using System.Collections.Generic;

namespace Assyst.Models
{
    /// <summary>
    /// Группа событий
    /// </summary>
    public class LinkedEventGroupItem
    {
        /// <summary>Id</summary>
        public long id { get; set; }
        public string entityDefinitionType { get; set; }
        public bool objectAvailable { get; set; }
        public bool systemRecordFlag { get; set; }
        public string assystUserId { get; set; }
        public DateTime linkDate { get; set; }
        public string linkGroupRemarks { get; set; }
        public long linkReasonId { get; set; }
        public string linkReasonName { get; set; }
    public List<LinkedEventItem> linkedEvents { get; set; }
    }
}
