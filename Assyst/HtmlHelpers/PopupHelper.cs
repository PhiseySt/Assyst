using System.Text.Encodings.Web;
using Assyst.Models;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Bootstrap;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assyst.HtmlHelpers
{
    public static class PopupHelper
    {
        public static HtmlString CreatePopup(
            this IHtmlHelper helper,
            PopupSettings popupSettings
            )
        {
            var div = new TagBuilder("div");
            div.AddCssClass("popup");
            div.Attributes.Add("name", popupSettings.name);

            var popup = helper.DevExpress()
                .BootstrapPopupControl("popup_" + popupSettings.name)
                .Width(popupSettings.width)
                .Height(popupSettings.height)
                .ShowHeader(true)
                .ShowFooter(popupSettings.showFooter)
                .ShowCloseButton(true)
                .PopupHorizontalAlign(PopupHorizontalAlign.WindowCenter)
                .PopupVerticalAlign(PopupVerticalAlign.WindowCenter)
                .AllowDragging(true)
                .AllowResize(true)
                .Modal(true)
                .ShowOnPageLoad(false)
                .ContentUrl(popupSettings.contentUrl)
                .HeaderText(popupSettings.headerText)
                .ClientSideEvents(events => events
                    .Closing("function(){top.window['" + popupSettings.name + "'].popup_Closing(" + popupSettings.name + ")}")
                    .Init("function(){top.window['" + popupSettings.name + "'].popup_Init(" + popupSettings.name + ")}")
                )
                .FooterContentTemplate(c =>
                {
                    var footer = new TagBuilder("text");
                    var submitButton = helper.DevExpress()
                        .BootstrapButton()
                        .Name("submitButton_" + popupSettings.name)
                        .Text("Сохранить")
                        .UseSubmitBehavior(true)
                        .SettingsBootstrap(settings => settings.RenderOption(BootstrapRenderOption.Primary))
                        .CssClasses(t => t.Control("btn-primary"))
                        .ClientSideEvents(e => e.Click("function(){top.window['" + popupSettings.name + "'].popup_SubmitButtonClick(" + popupSettings.name + ");}"));
                    var closeButton = helper.DevExpress()
                        .BootstrapButton()
                        .Name("closeButton" + popupSettings.name)
                        .Text("Отмена")
                        .CssClasses(t => t.Control("ml-2 btn-secondary"))
                        .SettingsBootstrap(settings => settings.RenderOption(BootstrapRenderOption.Primary))
                        .ClientSideEvents(e => e.Click("function(){top.window['" + popupSettings.name + "'].popup_CloseButtonClick(" + popupSettings.name + ");}"));
                    footer.InnerHtml.AppendHtml(submitButton);
                    footer.InnerHtml.AppendHtml(closeButton);
                    var writerf = new System.IO.StringWriter();
                    footer.WriteTo(writerf, HtmlEncoder.Default);
                    return new HtmlString(writerf.ToString());
                })
                .CloseAction(CloseAction.CloseButton);
            div.InnerHtml.AppendHtml(popup);

            var writer = new System.IO.StringWriter();
            div.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}
