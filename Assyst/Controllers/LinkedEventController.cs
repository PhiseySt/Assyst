using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {

        #region ActionResults

        [HttpPost]
        public long ActionAddLink(LinkEventItem item)
        {
            var crLinkedEventGroup = SaveLinkedEventGroup(item.linkedReasonId);
            var group = JsonConvert.DeserializeObject<LinkedEventGroupItem>(crLinkedEventGroup.Result);
            SaveLinkedEvent(item.eventId1, group.id);
            SaveLinkedEvent(item.eventId2, group.id);

            return group.id;
        }

        [Authorize]
        public IActionResult ActionAddLink(int eventId)
        {
            var item = new LinkEventItem()
            {
                eventId1 = eventId,
                event1 = GetEventById(eventId)
            };

            ViewBag.LinkedReasonList = GetLinkedReasons(new List<long>(){1,5,3,12,13,15});
            item.linkedReasonId = 3;

            return View(item);
        }

        [Authorize]
        public IActionResult LinkedEventsPartialGrid(long eventId)
        {
            List<LinkedEventGridItem> items = null;
            try
            {
                items = GetLinkedEvents(eventId);
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

        private List<LinkedEventGridItem> GetLinkedEvents(long eventId)
        {
            var linkedEventGridItems = new List<LinkedEventGridItem>();
            var linkedEventGroups = GetLinkedEventGroups(eventId);
            var linkedReasonList = GetLinkedReasons();
            foreach (var linkedEventGroup in linkedEventGroups)
            {
                var linkedReasonItem = linkedReasonList.FirstOrDefault(r => r.id == linkedEventGroup.linkReasonId);
                if (linkedReasonItem != null)
                    linkedEventGroup.linkReasonName = linkedReasonItem.name;
                if (linkedEventGroup.linkedEvents != null)
                    foreach (var linkEvent in linkedEventGroup.linkedEvents)
                        linkedEventGridItems.Add(new LinkedEventGridItem()
                        {
                            key = linkEvent.linkedEvent.id + "|" + linkEvent.linkedEvent.formattedReference + "|" + linkEvent.id + "|" + linkedEventGroup.id,
                            linkedEventGroupId = linkedEventGroup.id,
                            linkedEventGroup = linkedEventGroup,
                            linkedEvent = linkEvent.linkedEvent, 
                            linkedEventFormattedReference = linkEvent.linkedEvent.formattedReference
                        });
            }

            return linkedEventGridItems;
        }

        private void SaveLinkedEvent(long? eventId, long groupId)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                linkedEventGroupId = groupId,
                linkedEventId = eventId
            });
            var content = jsonBody;
            var url = AppConfig.HostUrl + AppConfig.GetUrlLink("LinkedEventGroups");
            var serviceUrl = url + groupId + "/events";
            var request = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
            {
                Content = new StringContent(
                  content,
                  Encoding.UTF8,
                  "application/json"
              )
            };
            SendRequest(request);
        }

        #endregion
    }
}
