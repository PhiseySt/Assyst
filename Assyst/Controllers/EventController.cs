using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Assyst.Extensions;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController : Controller
    {

        private static FixEventItem _fixEventRemark;

        public EventController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            if (CountSynchTread == 0)
            {
                StartSynch();
                CountSynchTread++;
            }
        }

        #region ActionResults

        [Authorize]
        public IActionResult Monitor(long? eventId)
        {
            if (eventId != null && eventId != 0)
            {
                var item = GetEventById((long)eventId);
                ViewData["eventId"] = item.id;
                ViewData["formattedReference"] = item.formattedReference;
                ViewData["Title"] = item.formattedReference;
            }
            else
            {
                ViewData["eventId"] = 0;
                ViewData["formattedReference"] = 0;
                ViewData["Title"] = "События";
            }

            return PartialView(null);
        }

        [Authorize]
        public IActionResult ContactUserEventsPartialGrid(long eventId)
        {
            List<EventItem> items = null;
            try
            {
                items = GetContactUserEvents(eventId);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }
            return PartialView(items);
        }

        [Authorize]
        [HttpPost]
        public IActionResult StackPartialGrid(long[] eventType, bool timerStop, bool myEvents, bool refresh)
        {
            List<EventItem> items = null;
            try
            {
                if (refresh)
                    items = GetStackEvents(eventType, timerStop, myEvents, false);

                items = GetStackEvents(eventType, timerStop, myEvents, refresh);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }

            return PartialView(items);
        }

        [Authorize]
        public IActionResult EventsIncidentAdd(long? affectedUserId, string remarks)
        {
            var item = new EventItem();
            SetViewBag(item, 1);

            var assystUser = GetAuthorizedAssystUser();
            item.assignedUser = assystUser;
            ViewData["assystUserList"] = GetAssystUserList(assystUser.servDeptId);

            if (affectedUserId != null)
            {
                item.affectedUser = GetContactUserById((long)affectedUserId);
                item.department = item.affectedUser?.department;
                item.room = item.affectedUser?.room;
            }

            if (remarks != null)
            {
                item.remarks = remarks;
                item.richRemarks = new RichRemarks()
                {
                    content = "<p>" + remarks + "</p>",
                    plainTextContent = remarks
                };
            }
            else
            {
                remarks = "<p>Номер магазина: S<br>Имя ПК(IP адрес):<br>Версия билда:<br>Описание проблемы:<br>Ошибка(текст ошибки дословно):<br>Приложен скриншот?:<br>Произведенные действия:<p>";
                item.remarks = remarks;
                item.richRemarks = new RichRemarks()
                {
                    content = remarks,
                    plainTextContent = remarks
                };
            }

            return View(item);
        }

        [Authorize]
        public IActionResult EventsConsultationAdd(int id, long eventId)
        {
            var parentEvent = GetEventById(eventId);
            var item = new EventItem
            {
                parentEventId = eventId,
                parentEvent = parentEvent,
                affectedUser = parentEvent.affectedUser,
                modeRead = 1,
                remarks = "Консультация: создание на основе события " + parentEvent.formattedReference,
                richRemarks = new RichRemarks()
                {
                    content = "<p>Консультация: создание на основе события " + parentEvent.formattedReference + "</p>",
                    plainTextContent = "Консультация: создание на основе события " + parentEvent.formattedReference,
                },
                categoryId = 66,
                itemAId = parentEvent.itemA.id
            };
            SetViewBag(item, 2);
            ViewData["Categories"] = GetCategoryList(item.itemAId, new List<long> { 22, item.categoryId });
            return View(item);
        }

        [Authorize]
        public IActionResult EventsComplaintAdd(int id, long eventId)
        {
            var parentEvent = GetEventById(eventId);
            var item = new EventItem
            {
                parentEventId = eventId,
                parentEvent = parentEvent,
                modeRead = 1,
                remarks = "Жалоба: создание на основе события " + parentEvent.formattedReference,
                richRemarks = new RichRemarks()
                {
                    content = "<p>Жалоба: создание на основе события " + parentEvent.formattedReference + "</p>",
                    plainTextContent = "Жалоба: создание на основе события " + parentEvent.formattedReference,
                },
                categoryId = 65,
                itemAId = parentEvent.itemA.id
            };
            SetViewBag(item, 3);
            ViewData["Categories"] = GetCategoryList(item.itemAId, new List<long> { 22, item.categoryId });
            return View(item);
        }

        [Authorize]
        public IActionResult EventsEdit(long eventId)
        {
            var item = GetEventById(eventId);

            ViewData["EditFormTemplateId"] = GetEditFormTemplateId(item);
            ViewData["Actions"] = item.actions;
            ViewData["LinkedEvents"] = GetLinkedEvents(eventId);
            ViewData["AffectedUserEvents"] = GetContactUserEvents(eventId);
            ViewData["Attachments"] = item.attachments;

            ViewData["Categories"] = GetCategoryList(item.itemAId, new List<long> { 22, item.categoryId });
            SetViewBag(item, 1);
            return View(item);
        }
        [HttpPost]
        public string EventsIncidentAdd(EventItem item)
        {
            if (item.affectedUserTelephone == null) item.affectedUserTelephone = "-";
            if (item.affectedUserTelephoneExtension == null) item.affectedUserTelephoneExtension = "-";
            return SaveEventItemResult(item, HttpMethod.Post);
        }

        [HttpPost]
        public string EventsConsultationAdd(EventItem item)
        {
            return SaveEventItemResult(item, HttpMethod.Post);
        }

        [HttpPost]
        public string EventsComplaintAdd(EventItem item)
        {
            return SaveEventItemResult(item, HttpMethod.Post);
        }

        [HttpPost]
        public void EventsEdit(EventItem item)
        {
            // В RestApi Assyst есть какой-то баг, что запросы на редактрование работают только со второй попытки (меняют данные)
            //в том числе через утилиты типа Postman
            //при этом при любой попытке  возвращается успешный ответ HTTP 200
            // поэтому здесь установлен двойной вызов SaveEventItemResult
            // проблема пропала, закомментировал
            SaveEventItemResult(item, HttpMethod.Put);
            //SaveEventItemResult(item, HttpMethod.Put);
        }

        [Authorize]
        public IActionResult SearchEventsPartialGrid(string formattedReference, long? affectedUserId, long? buildingId, string remarks)
        {
            List<EventItem> items = null;
            try
            {
                items = GetEvents(formattedReference, affectedUserId, buildingId, remarks);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }

            return PartialView(items);
        }

        public IActionResult Events(long? linkEventId)
        {
            ViewData["linkEventId"] = linkEventId;
            return View(new List<EventItem>());
        }

        [Authorize]
        public IActionResult EventsPartialGrid(string formattedReference, long? linkEventId)
        {
            List<EventItem> items = null;
            try
            {
                items = GetEvents(formattedReference, linkEventId);
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

        private List<EventItem> GetEvents(bool cacheRefresh = false)
        {
            List<EventItem> events;
            if (!_cache.TryGetValue("events", out events) || cacheRefresh)
            {
                var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("GetEvents");
                var client = InitHttpClient();
                var timeStartRequest = DateTime.Now;
                var task = client.GetAsync(serviceUrl).ContinueWith(requestTask =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait(AppConfig.HttpWaitResponceTime);
                    events = JsonConvert.DeserializeObject<List<EventItem>>(json.Result);
                });
                task.Wait(AppConfig.HttpWaitResponceTime);
                if (events.Any())
                {
                    InitCache();
                    // кэш открытых событий
                    _cache?.Set("events", events,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.CacheStorageTime));
                    CacheTimeLastSynchStart = timeStartRequest;
                }
            }
            if (events.Count > 0)
                events =
                    events.Where(
                        e => !string.IsNullOrEmpty(e.formattedReference) &&
                            e.formattedReference != "0").ToList();
            return events;
        }

        private EventItem GetEventById(long id)
        {
            EventItem eventItem = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetEventById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                eventItem = JsonConvert.DeserializeObject<EventItem>(json.Result);
            });
            task.Wait();
            if (eventItem != null)
            {
                if (eventItem.totalAttachmentCount > 0)
                {
                    // для файлов прикрепленных к событию генерируем ссылку для скачивания
                    if (eventItem.attachments != null)
                    {
                        foreach (var attachmentItem in eventItem.attachments)
                        {
                            attachmentItem.urldownload = GenerateEventAttachmentLink(eventItem.id, attachmentItem);
                        }
                    }
                    // для файлов прикрепленных к действию генерируем ccылку и привязываем их к событию
                    // так как по пожеланию заказчика все файлы должны отображаться на одной вкладке
                    var actionAttachments = eventItem?.actions.Where(p => p.attachments != null).Select(r => r);
                    foreach (var attachmentItem in actionAttachments)
                    {
                        var urlAttachment = GenerateActionAttachmentLink(attachmentItem.id, attachmentItem.attachments);
                        if (attachmentItem.attachments!=null) {
                        var attachActionToEvent = new AttachmentItem
                        {
                            urldownload = urlAttachment,
                            filename = attachmentItem.attachments.First().filename,
                            name = attachmentItem.attachments.First().name,
                            description = attachmentItem.attachments.First().description
                        };
                        if (eventItem.attachments != null) eventItem.attachments.Add(attachActionToEvent);
                        else
                        {
                            eventItem.attachments = new List<AttachmentItem> { attachActionToEvent };
                        }
                    }
                }
                }

                SetCacheEvent(eventItem);
            }

            return eventItem;
        }

        private List<EventItem> GetStackEvents(long[] eventType, bool timerStop, bool myEvents, bool refresh)
        {
            var items = GetEvents(refresh);

            // зафиксируем заданные ползователем фильтры в кэше
            var currentUser = GetAuthorizedAssystUser();
            var filterModel = new FilterModel
            {
                userName = currentUser.name,
                fltrMyEvents = myEvents,
                fltrTimerStop = timerStop,
                fltrEventType = eventType
            };
            SetCacheFilterValByUserId(filterModel);
            // получим события для монитора согласно заданным фильтрам
            if (eventType.Length == 0) return null;

            items = timerStop ?
                    items.Where(d => eventType.Contains(d.eventType) && d.lastSlaClockStop != null).ToList() : GetEvents(refresh).Where(d => eventType.Contains(d.eventType)).ToList();

            if (myEvents)
            {
                items = items.Where(d => d.assignedUserId == currentUser.id).ToList();
                return items;
            }

            return items;
        }

        private List<EventItem> GetContactUserEvents(long eventId)
        {
            var eventItem = GetEvents().FirstOrDefault(e => e.id == eventId);
            var userEvents = GetEvents().Where(es => eventItem != null && es.affectedUserId == eventItem.affectedUserId).ToList();
            return userEvents;
        }

        private void SetViewBag(EventItem item, int id = 0)
        {
            _fixEventRemark = null;
            List<SeriousnessItem> listSeriousnessItem;
            _cache.TryGetValue("seriousnesses", out listSeriousnessItem);
            ViewBag.SeriousnessList = listSeriousnessItem;
            List<PrioritityItem> listPrioritityItem;
            _cache.TryGetValue("priorities", out listPrioritityItem);
            ViewBag.PrioritityList = listPrioritityItem;
            List<ServiceDepartmentItem> listServiceDepartmentItem;
            _cache.TryGetValue("serviceDepartments", out listServiceDepartmentItem);
            ViewBag.ServiceDepartmentList = listServiceDepartmentItem;
            switch (id)
            {
                case (int)EIncidentType.Incident:
                    break;
                case (int)EIncidentType.Consultation:
                    ViewBag.LinkedReasonList = GetLinkedReasons();
                    _fixEventRemark = new FixEventItem
                    {
                        id = item.id,
                        remarks = item.remarks,
                        incidentType = EIncidentType.Consultation
                    };
                    ViewBag.FixEventRemark = _fixEventRemark;
                    break;
                case (int)EIncidentType.Complaint:
                    ViewBag.LinkedReasonList = GetLinkedReasons();
                    _fixEventRemark = new FixEventItem
                    {
                        id = item.id,
                        remarks = item.remarks,
                        incidentType = EIncidentType.Complaint
                    };
                    ViewBag.FixEventRemark = _fixEventRemark;
                    break;
            }
        }

        private Task<string> SaveEventById(EventItem item, HttpMethod method)
        {
            string requestUri = null;
            // режим modeRead == 1 означает, что поля заполняются на основе данных родительского события (создание жалоб/консультаций)
            if (item.modeRead == 1)
            {
                if (item.parentEventId != null)
                {
                    var parentEvent = GetEventById((long)item.parentEventId);

                    var assystUser = GetAuthorizedAssystUser();
                    item.assignedServDeptId = assystUser.servDeptId;
                    item.assignedUserId = assystUser.id;
                    item.eventStatus = parentEvent.eventStatus;
                    item.eventType = parentEvent.eventType;
                    item.itemAId = parentEvent.itemAId;
                    item.itemBId = parentEvent.itemBId;
                    item.parentEventId = parentEvent.id;

                    var eventBuilder = GetEventBuilderByCategoryId(parentEvent.categoryId, parentEvent.itemAId);
                    if (eventBuilder != null)
                    {
                        item.seriousnessId = eventBuilder.seriousnessId;
                        item.priorityId = eventBuilder.priorityId;
                    }
                    else
                    {
                        item.seriousnessId = parentEvent.seriousnessId;
                        item.priorityId = parentEvent.priorityId;
                    }

                    var contactUser = GetContactUserById(item.affectedUserId);
                    if (contactUser.id > 0 && contactUser.id != parentEvent.affectedUserId)
                    {
                        item.affectedUserId = contactUser.id;
                        item.affectedUserTelephone = !string.IsNullOrEmpty(contactUser.officeTelephone) ? contactUser.officeTelephone : "-";
                        item.affectedUserTelephoneExtension = !string.IsNullOrEmpty(contactUser.officeTelephoneExtension) ? contactUser.officeTelephoneExtension : "-";
                        item.departmentId = contactUser.departmentId;
                        item.roomId = contactUser.roomId;
                    }
                    else
                    {
                        item.affectedUserId = parentEvent.affectedUserId;
                        item.affectedUserTelephone = parentEvent.affectedUserTelephone;
                        item.affectedUserTelephoneExtension = parentEvent.affectedUserTelephoneExtension;
                        item.departmentId = parentEvent.departmentId;
                        item.roomId = parentEvent.roomId;
                    }
                }
            }

            string content = null;
            var url = AppConfig.HostUrl + AppConfig.GetUrlLink("Events");

            // создание нового события
            if (method == HttpMethod.Post)
            {
                content = JsonConvert.SerializeObject(new
                {
                    // упорядочиваем поля для читаемости по алфавиту
                    item.affectedUserId,
                    item.affectedUserTelephone,
                    item.affectedUserTelephoneExtension,
                    item.assignedServDeptId,
                    item.assignedUserId,
                    item.categoryId,
                    item.departmentId,
                    item.itemAId,
                    item.itemBId,
                    item.parentEventId,
                    item.priorityId,
                    item.remarks,
                    item.richRemarks,
                    item.roomId,
                    item.seriousnessId,
                    item.eventType
                });
                requestUri = url;
            }
            // редактирование события
            if (method == HttpMethod.Put)
            {
                switch ((EEventType)item.eventType)
                {
                    case EEventType.INCIDENT:
                        content = JsonConvert.SerializeObject(new
                        {
                            // упорядочиваем поля для читаемости по алфавиту
                            item.affectedUserId,
                            item.affectedUserTelephone,
                            item.affectedUserTelephoneExtension,
                            item.categoryId,
                            item.departmentId,
                            item.itemAId,
                            item.itemBId,
                            item.parentEventId,
                            item.priorityId,
                            item.remarks,
                            item.richRemarks,
                            item.roomId,
                            item.seriousnessId
                        });
                        break;
                    case EEventType.PROBLEM:
                        content = JsonConvert.SerializeObject(new
                        {
                        });
                        break;
                    case EEventType.CHANGE:
                        content = JsonConvert.SerializeObject(new
                        {
                            // упорядочиваем поля для читаемости по алфавиту
                            item.affectedUserTelephone,
                            item.affectedUserTelephoneExtension,
                            item.remarks,
                            item.richRemarks
                        });
                        break;
                    case EEventType.NORMALTASK:
                    case EEventType.DECISIONTASK:
                    case EEventType.AUTHORISATIONTASK:
                        content = JsonConvert.SerializeObject(new
                        {
                            // упорядочиваем поля для читаемости по алфавиту
                            item.remarks,
                            item.richRemarks
                        });
                        break;
                }

                requestUri = url + item.id;
            }
            // Assyst принимает метод HttpMethod.Post и для создания и для редактирования
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            var contentsResponce = SendRequest(request);
            if (item.modeRead == 1)
            {
                var idEvent = JsonConvert.DeserializeObject<EventItem>(contentsResponce.Result);
                var crLinkedEventGroup = SaveLinkedEventGroup(item.linkedReasonId);
                var groupNo = JsonConvert.DeserializeObject<LinkedEventGroupItem>(crLinkedEventGroup.Result);
                SaveLinkedEvent(item.parentEventId, groupNo.id);
                SaveLinkedEvent(idEvent.id, groupNo.id);
            }
            return contentsResponce;
        }

        private Task<string> SendRequest(HttpRequestMessage request)
        {
            Task<string> contentsResponce = null;
            var client = InitHttpClient();
            var task = client.SendAsync(request).ContinueWith((responseTask) =>
            {
                var resultMessage = GetExceptionMessage(responseTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = responseTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                contentsResponce = response.Content.ReadAsStringAsync();
                return contentsResponce;
            });
            task.Wait();
            return contentsResponce;
        }




        private string SaveEventItemResult(EventItem item, HttpMethod method)
        {
            var contentsResponce = SaveEventById(item, method);
            var eventResult = JsonConvert.DeserializeObject<EventItem>(contentsResponce.Result);
            var eventItem = GetEventById(eventResult.id);
            SetCacheEvent(eventItem);
            return contentsResponce.Result;
        }

        public HttpClient InitHttpClient()
        {
            var client = new HttpClient { Timeout = AppConfig.HttpWaitResponceTime };
            var autorizationData = GetAuthorizationData();
            var tokenAuthorization = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(autorizationData));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", tokenAuthorization);
            return client;
        }

        private string GetAuthorizationData()
        {
            var autorizationData = User?.Identity?.Name;
            return autorizationData;
        }

        private List<EventItem> GetEvents(string formattedReference, long? affectedUserId, long? buildingId, string remarks)
        {
            formattedReference = formattedReference?.Trim();
            remarks = remarks?.Trim();

            List<EventItem> items = new List<EventItem>();

            var eventId = EventExtensions.GetEventIdByFormattedReference(formattedReference);
            if (eventId == null &&
                affectedUserId == null &&
                buildingId == null &&
                string.IsNullOrEmpty(remarks)
                )
                return items;

            var queryParams = new Dictionary<string, string>();
            if (eventId != null) queryParams.Add("eventId", eventId.ToString());
            if (affectedUserId != null) queryParams.Add("affectedUserId", affectedUserId.ToString());
            if (buildingId != null) queryParams.Add("buildingId", buildingId.ToString());
            if (!string.IsNullOrEmpty(remarks)) queryParams.Add("remarks[like]", "%" + remarks + "%");
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetSearchEvents"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<EventItem>>(json.Result);
            });
            task.Wait();

            InitCache();
            return items;
        }

        private List<EventItem> GetEvents(string formattedReference, long? linkEventId)
        {
            formattedReference = formattedReference?.Trim();

            List<EventItem> items = new List<EventItem>();

            var eventId = EventExtensions.GetEventIdByFormattedReference(formattedReference);
            if (eventId == null || eventId == linkEventId) return items;

            var queryParams = new Dictionary<string, string>();
            queryParams.Add("eventId", eventId.ToString());
            var serviceUrl =
                QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetSearchEvents"),
                    queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<EventItem>>(json.Result);
            });
            task.Wait();
            return items;
        }

        private static int GetEditFormTemplateId(EventItem item)
        {
            var editFormTemplateId = 1;
            switch ((EEventType)item.eventType)
            {
                case EEventType.INCIDENT:
                    editFormTemplateId = 1;
                    break;
                case EEventType.PROBLEM:
                    editFormTemplateId = 2;
                    break;
                case EEventType.CHANGE:
                    editFormTemplateId = 3;
                    break;
                case EEventType.NORMALTASK:
                case EEventType.DECISIONTASK:
                case EEventType.AUTHORISATIONTASK:
                    editFormTemplateId = 4;
                    break;
            }
            return editFormTemplateId;
        }

        #endregion

    }

}