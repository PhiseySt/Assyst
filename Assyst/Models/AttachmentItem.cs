using System;
using System.Collections.Generic;

namespace Assyst.Models
{
    /// <summary>
    /// Вложение файла к событию
    /// </summary>

    public class AttachmentItem
    {
        // Идентификатор вложения
        public long id { get; set; }
        // Описание вложения
        public string description { get; set; }
        // Название вложения
        public string filename { get; set; }
        // Имя файла
        public string name { get; set; }
        // Ссылка для скачивания
        public string urldownload { get; set; }
        // Бинарный код вложения
        public string attachment { get; set; }
    }


    /// <summary>
    /// Вложение файла для xml (загрузка файла через )
    /// </summary>

    public class attachment
    {
  // Описание вложения
        public string description { get; set; }
        // Название вложения
        public string fileName { get; set; }
        // Имя файла
        public string name { get; set; }
        // Бинарный код вложения
        public string attachmentInner { get; set; }
    }
}
