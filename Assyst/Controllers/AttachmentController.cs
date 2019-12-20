using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Assyst.Controllers
{
    public partial class EventController
    {

        #region ActionResults

        [Authorize]
        public IActionResult AttachmentsPartialGrid(long eventId)
        {
            List<AttachmentItem> items = null;
            try
            {
                items = GetAttachmentByEventId(eventId);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }
            return PartialView(items);
        }

        #endregion

        #region Properties & Methods

        private string GenerateEventAttachmentLink(long eventId, AttachmentItem attachmentItem)
        {
            return AppConfig.HostUrl + AppConfig.GetUrlLink("Events") + eventId + "/attachments/" + attachmentItem.id + "/binary";
        }

        private string GenerateActionAttachmentLink(long actionId, AttachmentItem[] attachmentItem)
        {
            if (attachmentItem is null) return "";

            return AppConfig.HostUrl + AppConfig.GetUrlLink("AddAction") + actionId + "/attachments/" + attachmentItem.First().id + "/binary";
        }

        private List<AttachmentItem> GetAttachmentByEventId(long eventId)
        {
            // этот метод нужен для получения вложений для event
            var attachItems = new List<AttachmentItem>();
            var serviceUrl = AppConfig.HostUrl + $"events/{eventId}/attachments";
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                attachItems = JsonConvert.DeserializeObject<List<AttachmentItem>>(json.Result);
            });
            task.Wait();
            foreach (var attachmentItem in attachItems)
            {
                attachmentItem.urldownload = GenerateEventAttachmentLink(eventId, attachmentItem);
            }

            return attachItems;
        }

        private string SaveAttachmentResult(ActionItem item)
        {
            var contentsResponce = SaveAttachment(item);
            return contentsResponce.Result;
        }

        private Task<string> SaveAttachment(ActionItem item)
        {
            // загрузуку файла на сервер (uploading) делаем в формате application/xml, так как restapi похоже не поддерживает
            //для этой операции формат application/json
            var attachment = new attachment
            {
                attachmentInner = item.attach.attachment,
                fileName = item.attach.filename,
                name = item.attach.name,
                description = item.attach.description
            };

            string content;
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(attachment.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, attachment, emptyNamespaces);
                content = stream.ToString();
            }

            var correctContent = content.Replace("attachmentInner", "attachment");
            var url = AppConfig.HostUrl + $"events/{item.eventId}/attachments";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    correctContent,
                    Encoding.UTF8,
                    "application/xml"
                )
            };
            var contentsResponce = SendRequest(request);
            return contentsResponce;
        }

        #endregion
    }
}