using System;
using System.Collections.Generic;

namespace Assyst.Models
{
    /// <summary>
    /// Cвязанное событие
    /// </summary>
    public class LinkedEventItem
    {
        /// <summary>Id связанного события</summary>
        public long id { get; set; }
        public EventItem linkedEvent { get; set; }
    }

    public class LinkedEventGridItem 
    {
        public string key { get; set; }
        public long linkedEventGroupId { get; set; }
        public string linkedEventFormattedReference { get; set; }
        public EventItem linkedEvent { get; set; }
        public LinkedEventGroupItem linkedEventGroup { get; set; }

    }

    /// <summary>
    /// Cвязывание событий
    /// </summary>
    public class LinkEventItem
    {
        /// <summary>Id 1 события</summary>
        public long eventId1 { get; set; }
        /// <summary>Id 2 события</summary>
        public long eventId2 { get; set; }
        /// <summary>Причина связи</summary>
        public long linkedReasonId { get; set; }
        /// <summary>1 события</summary>
        public EventItem event1 { get; set; }
        /// <summary>2 события</summary>
        public EventItem event2 { get; set; }
    }
}
