using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        [Authorize]
        public IActionResult ActionsPartialGrid(long eventId)
        {
            List<ActionItem> items = null;
            try
            {
                items = GetActionsByEventId(eventId);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }
            return PartialView(items);
        }

        [Authorize]
        public IActionResult ActionSimpleFormAdd(long eventId, long actionTypeId)
        {
            var item = new ActionItem
            {
                actionType = new ActionTypeItem { id = actionTypeId },
                eventId = eventId
            };
            if (actionTypeId == 14) // Если тип действия - решено поставщиком
            {
                item.richRemarks = new RichRemarks()
                {
                    content = "<p>Выездной специалист<br>Фамилия Имя Отчество:<br>Название организации:<br>Контактный номер(необязательно):<br>Номер заявки, по которой проводились работы:<br>Выполненные действия по заявке:<br><br>Cотрудник магазина<br>Магазин:<br>Табельный номер:<br>Фамилия Имя Отчество:<br>Качеством выполненных работ доволен<br>Работы принял сотрудник<br>Фамилия Имя Отчество:</p>",
                    plainTextContent = null
                };
            }
            return View(item);
        }

        [Authorize]
        public IActionResult ActionAssignExecutionFormAdd(long eventId, long actionTypeId)
        {
            ViewBag.CauseCategoryList = GetCauseCategoryList();
            ViewBag.ServiceDepartmenList = GetServiceDepartmentList();

            var item = new ActionItem
            {
                actionType = new ActionTypeItem { id = actionTypeId },
                eventId = eventId
            };

            return View(item);
        }

        [Authorize]
        public IActionResult ActionHasExecutedFormAdd(long eventId, long actionTypeId, long assignedServDeptId, long causeItemId, string causeItemName)
        {
            var item = new ActionItem {
                actionType = new ActionTypeItem
                {
                    id = actionTypeId
                },
                eventId = eventId,
                assignedServDeptId  = assignedServDeptId,
                assignedServDept = new ServiceDepartmentItem()
                {
                    id = assignedServDeptId,
                    name = GetNoToNameServiceDepartments(assignedServDeptId)
                },
                causeItemId = causeItemId,
                causeItem = new ObjectItem
                {
                    id = causeItemId,
                    name = causeItemName
                },
                causeCategory = GetCategoryById(27)
            };
            return View(item);
        }

        [Authorize]
        public IActionResult ActionAddAttachment(long eventId, long actionTypeId)
        {
            var item = new ActionItem
            {
                actionType = new ActionTypeItem { id = actionTypeId },
                eventId = eventId
            };
            return View(item);
        }

        [HttpPost]
        public void ActionSimpleFormAdd(ActionItem item) => SaveActionItemResult(item);

        [HttpPost]
        public void ActionAssignExecutionFormAdd(ActionItem item) => SaveActionItemResult(item);

        [HttpPost]
        public void ActionHasExecutedFormAdd(ActionItem item) => SaveActionItemResult(item);
        [HttpPost]
        public void ActionAddAttachment(ActionItem item) => SaveAttachmentResult(item);

        #endregion

        #region Events Properties & Methods Actions

        private List<ActionItem> GetActionsByEventId(long id)
        {
            var eventItem = GetEventById(id);
            var actions = eventItem.actions;
            return actions;
        }

        // текст сообщения при 'остановке таймера заявки'
        public const string StopTimerMessage = "Таймер заявки остановлен";

        private void SaveActionItemResult(ActionItem item)
        {
            var contentsResponce = SaveAction(item);
            ActionItem actionItem = JsonConvert.DeserializeObject<ActionItem>(contentsResponce.Result);
            // остановка процесса пока новое действие не появится в списке действий события (если более 20сек, отпускаем)
            var i = 0;
            while (true)
            {
                var eventItem = GetEventById(actionItem.eventId);
                if (eventItem.actions.Any(r => r.id == actionItem.id)) break;
                i++;
                if(i > 20) break;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private Task<string> SaveAction(ActionItem item)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                item.actionType,
                item.assignedServDeptId,
                item.assignedUserId,
                item.causeCategoryId,
                item.causeItemId,
                item.eventId,
                item.remarks,
                item.richRemarks
            },
                Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var content = jsonBody;
            var url = AppConfig.HostUrl + AppConfig.GetUrlLink("AddAction");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json"
                )
            };
            var contentsResponce = SendRequest(request);
            return contentsResponce;
        }

        #endregion
    }
}