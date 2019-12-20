using System.ComponentModel;

namespace Assyst.Models
{
    /// <summary>
    /// Класс настройки сообщения
    /// </summary>
    public class ViewMessageSettings
    {
        public ViewMessageSettings()
        {
            name = string.Empty;
            type = MessageType.None;
            text = string.Empty;
            title = string.Empty;
        }

        public ViewMessageSettings(string name)
        {
            this.name = name;
            type = MessageType.None;
            text = string.Empty;
            title = string.Empty;
        }
        public ViewMessageSettings(string name, MessageType type, string text, string title)
        {
            this.name = name;
            this.type = type;
            this.text = text;
            this.title = title;
        }
        public string name { get; set; }
        public MessageType type { get; set; }
        public string text { get; set; }
        public string title { get; set; }
    }

    public enum MessageType
    {
        [Description("None")]
        None,
        [Description("Information")]
        Information,
        [Description("Warning")]
        Warning,
        [Description("Error")]
        Error,
        [Description("Success")]
        Success
    }
}
