
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Assyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        // Id объекта с названием "Объект не указан"
        const long ObjectNotSpecifiedId = 174091;

        #region ActionResults

        public IActionResult Objects(string name, long? relatedItemId)
        {
            ViewData["name"] = name;
            ViewData["relatedItemId"] = relatedItemId != ObjectNotSpecifiedId ? relatedItemId : null;
            return View(new List<ObjectItem>());
        }

        public IActionResult ObjectsPartialGrid(string name, long? relatedItemId)
        {
            List<ObjectItem> items = null;
            try
            {
                items = GetObjectList(name, relatedItemId);
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

        public string GetJsonObjectById(long id)
        {
            return JsonConvert.SerializeObject(GetObjectById(id));
        }

        private ObjectItem GetObjectById(long id)
        {
            ObjectItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetObjectById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<ObjectItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameObjects(long value)
        {
            string name = null;
            var item = GetObjectById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<ObjectItem> GetObjectList(string name, long? relatedItemId)
        {
            name = name?.Trim();

            List<ObjectItem> items = new List<ObjectItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name) && name != "null") queryParams.Add("name[like]", "%" + name + "%");
            if (relatedItemId != null) queryParams.Add("relatedItemId", relatedItemId.ToString());
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetObjects"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<ObjectItem>>(json.Result);
            });
            task.Wait();
            
            return items;
        }

        #endregion
    }
}
