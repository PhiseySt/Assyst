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

        public IActionResult Rooms(string name)
        {
            ViewData["name"] = name;
            return View(new List<RoomItem>());
        }

        public IActionResult RoomsPartialGrid(string name, long? buildingId)
        {
            List<RoomItem> items = null;
            try
            {
                items = GetRoomList(name, buildingId);
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

        public string GetJsonRoomById(long id)
        {
            return JsonConvert.SerializeObject(GetRoomById(id));
        }

        private RoomItem GetRoomById(long id)
        {
            var item = GetRoomList().FirstOrDefault(e => e.id == id);
            if (item == null)
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetRoomById"), id);
                var client = InitHttpClient();
                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    item = JsonConvert.DeserializeObject<RoomItem>(json.Result);
                });
                task.Wait();
            }
            return item;
        }

        public string GetNoToNameRooms(long value)
        {
            string name = null;
            var item = GetRoomById(value);
            if (item != null)
                name = item.roomName;
            return name;
        }

        private List<RoomItem> GetRoomList()
        {
            List<RoomItem> itemsCache;
            if (!_cache.TryGetValue("items", out itemsCache))
            {
                return new List<RoomItem>();
            }

            return itemsCache;
        }

        private List<RoomItem> GetRoomList(string name, long? buildingId)
        {
            name = name?.Trim();

            List<RoomItem> items = new List<RoomItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name) && name!="null") queryParams.Add("name[like]", "%" + name + "%");
            if (buildingId != null) queryParams.Add("buildingId", buildingId.ToString());
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetRooms"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<RoomItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
