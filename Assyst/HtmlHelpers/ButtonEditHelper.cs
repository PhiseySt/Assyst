using System;
using System.Linq;
using System.Text.Encodings.Web;
using DevExpress.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Assyst.HtmlHelpers
{
    public static class ButtonEditHelper
    {
        public static HtmlString CreateButtonEdit(
            this IHtmlHelper helper,
            string name,
            string view,
            string headerText,
            long? value = null,
            string text = null,
            object attributes = null
            )
        {
            var dicAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
            var arrAttributes = "{";
            arrAttributes += string.Join(",", dicAttributes.Select(k => "'" + k.Key + "':" + (IsNumeric(k.Value)? k.Value : "'" + k.Value + "'")).ToArray());
            arrAttributes += "}";

            var div = new TagBuilder("div");
            div.AddCssClass("buttonEdit");

            var linkCss = new TagBuilder("link");
            linkCss.Attributes["rel"] = "stylesheet";
            linkCss.Attributes["src"] = "/css/buttonEdit.css";
            div.InnerHtml.AppendHtml(linkCss);

            var scriptJs = new TagBuilder("script");
            scriptJs.Attributes["type"] = "text/javascript";
            scriptJs.Attributes["src"] = "/js/buttonEdit.js?j=13";
            div.InnerHtml.AppendHtml(scriptJs);

            var script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            var scriptBody = "var buttonEdit_" + name + " = new ButtonEdit("+
                "'" + name + "'," +
                "'" + view + "'," +
                "'" + headerText + "'," +
                (value!=null ? value.ToString() : "null") + "," +
                (text !=null ? "'" + text + "'"  : "null") + "," +
               (dicAttributes.Count > 0 ?  arrAttributes : "new Array()") + ")";
            script.InnerHtml.AppendHtml(scriptBody);
            div.InnerHtml.AppendHtml(script);

            var hdf = helper.DevExpress()
                .BootstrapTextBox(name)
                .Value(value)
                .ClientVisible(false)
                .ClientSideEvents(events => events
                    .Init("function(){buttonEdit_" + name + ".hdf_Init(" + name + ")}")
                    .ValueChanged("function(){hdf_ValueChanged()}")
                    )
                ;
            div.InnerHtml.AppendHtml(hdf);

            var be = helper.DevExpress()
                .BootstrapButtonEdit("be_" + name)
                .ClearButton(clearButton => clearButton
                    .DisplayMode(ClearButtonDisplayMode.OnHover))
                .ClientSideEvents(events => events
                    .Init("function(){buttonEdit_" + name + ".be_Init(be_" + name + ")}")
                    .ButtonClick("function(){buttonEdit_" + name + ".be_ButtonClick()}")
                    .ValueChanged("function(){buttonEdit_" + name + ".be_ValueChanged()}")
                    )
                .ReadOnly(false)
                .Value(text)
                .Buttons(buttons => buttons.Add());
            div.InnerHtml.AppendHtml(be);

            var writer = new System.IO.StringWriter();
            div.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}
