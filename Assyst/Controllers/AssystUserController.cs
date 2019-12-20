using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        [Authorize]
        public IActionResult AssystUsers(string name, long? assignedServDeptId)
        {
            ViewBag.ServiceDepartmenList = GetServiceDepartmentList();
            ViewData["name"] = name;
            ViewData["assignedServDeptId"] = assignedServDeptId;
            return View(new List<AssystUserItem>());
        }

        [Authorize]
        public IActionResult AssystUsersPartialGrid(string shortCode, string name, string servDeptId)
        {
            List<AssystUserItem> items = null;
            try
            {
                items = GetAssystUserList(shortCode, name, servDeptId);
                ViewData["result"] = "success";
            }
            catch (Exception e)
            {
                ViewData["result"] = e.InnerException?.Message ?? e.Message;
            }

            return PartialView(items);
        }

        [Authorize]
        public IActionResult AssystUsersPartialCb(long servDeptId)
        {
            return PartialView(GetAssystUserList(servDeptId));
        }

        #endregion

        #region Properties & Methods

        protected List<string> ListAssystUserId { get; set; } = new List<string>();

        protected static List<AssystUserItem> ListAssystUser { get; set; }

        private AssystUserItem GetAuthorizedAssystUser()
        {
            var identityName = User.Identity.Name;
            var shortCode = identityName.Substring(0, identityName.IndexOf(":", StringComparison.Ordinal));
            return GetAssystUserList(260).FirstOrDefault(u => u.shortCode.ToLower() == shortCode.ToLower());
        }

        public string GetJsonAssystUserById(long id)
        {
            return JsonConvert.SerializeObject(GetAssystUserById(id));
        }

        private AssystUserItem GetAssystUserById(long id)
        {
            AssystUserItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetAssystUserById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<AssystUserItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        private AssystUserItem GetAssystUserById(long id, long servDeptId)
        {
            var item = GetAssystUserList(servDeptId).FirstOrDefault(e => e.id == id);
            if (item == null)
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetAssystUserById"), id);
                var client = InitHttpClient();
                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    item = JsonConvert.DeserializeObject<AssystUserItem>(json.Result);
                });
                task.Wait();
            }
            return item;
        }

        public string GetNoToNameAssystUsers(long id, long servDeptId)
        {
            string name = null;
            var item = GetAssystUserById(id, servDeptId);
            if (item != null)
                name = item.name;
            return name;
        }

        public string GetNoToNameAssystUsers(long id)
        {
            string name = null;
            var item = GetAssystUserById(id);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<AssystUserItem> GetAssystUserList(long servDeptId)
        {
            List<AssystUserItem> items;
            if (!_cache.TryGetValue("assystusers[" + servDeptId + "]", out items))
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetAssystUsersByServDept"), servDeptId);
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<AssystUserItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    _cache?.Set("assystusers[" + servDeptId + "]", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }

        private List<AssystUserItem> GetAssystUserList(string shortCode, string name, string servDeptId)
        {
            List<AssystUserItem> items = new List<AssystUserItem>();

            shortCode = shortCode?.Trim();
            name = name?.Trim();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shortCode)) queryParams.Add("shortCode", shortCode);
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", "%" + name + "%");
            if (!string.IsNullOrEmpty(servDeptId)) queryParams.Add("servDeptId", servDeptId);
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetAssystUsers"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<AssystUserItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
