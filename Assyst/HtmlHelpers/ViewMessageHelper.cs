using System;
using System.Text;
using System.Text.Encodings.Web;
using Assyst.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Assyst.HtmlHelpers
{
    public static class ViewMessageHelper
    {
        public static HtmlString CreateMessageView(this IHtmlHelper helper, ViewMessageSettings messageSettings)
        {
            return CreateMessageView(helper, messageSettings, null);
        }

        public static HtmlString CreateMessageView(this IHtmlHelper helper, ViewMessageSettings messageSettings,
            object htmlAttributes)
        {
            return BuildMessageHelper(helper, messageSettings, htmlAttributes);
        }

        private static HtmlString BuildMessageHelper(this IHtmlHelper helper, ViewMessageSettings messageSettings,
            object attributes)
        {
            // Create the container
            var div = new TagBuilder("div");

            var script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            var scriptBody = "var " + messageSettings.name + " = new ViewMessage(" +
                "'" + messageSettings.name + "'," +
                "'" + messageSettings.type + "'," +
                "'" + messageSettings.text + "'," +
                "'" + messageSettings.title + "');";
            script.InnerHtml.AppendHtml(scriptBody);
            div.InnerHtml.AppendHtml(script);

            div.Attributes.Add("id", messageSettings.name);
            div.MergeAttribute("data-alert", "alert alert-sm");
            switch (messageSettings.type)
            {
                case MessageType.None:
                    div.MergeAttribute("class", "alert fade in  d-none");
                    break;
                case MessageType.Information:
                    div.MergeAttribute("class", "alert alert-info fade in d-none");
                    break;
                case MessageType.Error:
                    div.MergeAttribute("class", "alert alert-danger fade in d-none");
                    break;
                case MessageType.Warning:
                    div.MergeAttribute("class", "alert alert-warning fade in d-none");
                    break;
                case MessageType.Success:
                    div.MergeAttribute("class", "alert alert-success fade in d-none");
                    break;
            }

            div.MergeAttributes(new RouteValueDictionary(attributes));

            var sb = new StringBuilder();
            sb.Append("<a class=\"close cursor-pointer\"  onclick=\"" + messageSettings.name + ".hide(); return false;\">×</a>");
            sb.AppendFormat("<h4>{0}</h4>", messageSettings.text);
            sb.AppendFormat("<p>{0}</p>", messageSettings.text);
            div.InnerHtml.AppendHtml(sb.ToString());

            var writer = new System.IO.StringWriter();
            div.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}
