namespace Assyst.Models
{
    public class Entry
    {


    }

    /// <summary>
    /// Сущность логирования в систему
    /// </summary>
    public class EntryPoint
    {
        /// <summary>Идентификатор</summary>
        public string Id { get; set; }
        /// <summary>Имя пользователя</summary>
        public string UserName { get; set; }
        /// <summary>Время входа в систему</summary>
        public string TimeEntry { get; set; }

    }

}
