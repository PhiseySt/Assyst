using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Assyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        public IActionResult Buildings(string name)
        {
            ViewData["name"] = name;
            return View(new List<BuildingItem>());
        }

        public IActionResult BuildingsPartialGrid(string name)
        {
            List<BuildingItem> items = null;
            try
            {
                items = GetBuildingList(name);
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

        public string GetJsonBuildingById(long id)
        {
            return JsonConvert.SerializeObject(GetBuildingById(id));
        }

        private BuildingItem GetBuildingById(long id)
        {
            BuildingItem item = null;
            var serviceUrl = AppConfig.HostUrl + $"buildings/{id}";
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<BuildingItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameBuildings(long value)
        {
            string name = null;
            var item = GetBuildingById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<BuildingItem> GetBuildingList(string name)
        {
            name = name?.Trim();

            List<BuildingItem> items = new List<BuildingItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", "%" + name + "%");
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetBuildings"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<BuildingItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
