using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Assyst.Models
{
    /// <summary>
    /// Класс исключение
    /// </summary>
    public class ExceptionItem
    {
        /// <summary>key</summary>
        public string messageKey { get; set; }
        /// <summary>Сообщение</summary>
        public string message { get; set; }
        /// <summary>Тип</summary>
        public string type { get; set; }
        /// <summary>Диагностика</summary>
        public string diagnostic { get; set; }
        /// <summary>Ошибки</summary>
        public List<ErrorItem> errors { get; set; }

        public string htmlText
        {
            get
            {
                var msg = new StringBuilder();
                if (!string.IsNullOrEmpty(type))
                    msg.Append("тип: " +  type + "<br>");
                if (!string.IsNullOrEmpty(message))
                    msg.Append("cообщение: " + message + "<br>");
                if (!string.IsNullOrEmpty(diagnostic))
                    msg.Append("диагностика: " + diagnostic);
                if (errors != null && errors.Count > 0)
                {
                    foreach (ErrorItem error in errors)
                    {
                        msg.Append("<br>");
                        if (!string.IsNullOrEmpty(error.field))
                            msg.Append(error.field + ":");
                        if (!string.IsNullOrEmpty(error.message))
                            msg.Append(" " +  error.message);
                        if (!string.IsNullOrEmpty(diagnostic))
                            msg.Append(" " + error.diagnostic);
                    }
                }
                return HttpUtility.HtmlDecode(msg.ToString());
            }
        }

    }
}
