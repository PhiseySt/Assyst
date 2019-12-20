using System;
using System.Collections.Generic;
using System.Net.Http;
using Assyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        public IActionResult ContactUsers(string name)
        {
            ViewData["name"] = name;
            return View(new List<ContactUserItem>());
        }

        public IActionResult ContactUsersPartialGrid(string shortCode, string name, long? sectionId, long? buildingId)
        {
            List<ContactUserItem> items = null;
            try
            {
                items = GetContactUserList(shortCode, name, sectionId, buildingId);
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

        public string GetJsonContactUserById(long id)
        {
            return JsonConvert.SerializeObject(GetContactUserById(id));
        }

        private ContactUserItem GetContactUserById(long id)
        {
            ContactUserItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetContactUserById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<ContactUserItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameContactUsers(long value)
        {
            string name = null;
            var item = GetContactUserById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<ContactUserItem> GetContactUserList(string shortCode, string name, long? sectionId, long? buildingId)
        {

            shortCode = shortCode?.Trim();
            name = name?.Trim();

            List<ContactUserItem> items = new List<ContactUserItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shortCode)) queryParams.Add("shortCode", shortCode);
            if (!string.IsNullOrEmpty(name) && name != "null") queryParams.Add("name[like]", "%" + name + "%");
            if (sectionId != null) queryParams.Add("sectionId", sectionId.ToString());
            if (buildingId != null) queryParams.Add("buildingId", buildingId.ToString());
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetContactUsers"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<ContactUserItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
