
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

        public IActionResult ServiceDepartments()
        {
            return View(new List<ServiceDepartmentItem>());
        }

        public IActionResult ServiceDepartmentsPartialGrid(string shortCode, string name)
        {
            List<ServiceDepartmentItem> items = null;
            try
            {
                items = GetServiceDepartmentList(shortCode, name);
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

        protected static List<ServiceDepartmentItem> ListServiceDepartment { get; set; }

        private void InitServiceDepartmentData()
        {
            ListServiceDepartment = GetServiceDepartmentList();
        }

        private List<ServiceDepartmentItem> GetServiceDepartmentList(string shortCode, string name)
        {
            List<ServiceDepartmentItem> items = new List<ServiceDepartmentItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shortCode)) queryParams.Add("shortCode", shortCode);
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name", name);
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetProducts"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<ServiceDepartmentItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        public string GetNoToNameServiceDepartments(long value)
        {
            string name = null;
            var item = GetServiceDepartmentById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private ServiceDepartmentItem GetServiceDepartmentById(long id)
        {
            var item = GetServiceDepartmentList().FirstOrDefault(e => e.id == id);
            if (item == null)
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetServiceDepartmentById"), id);
                var client = InitHttpClient();
                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    item = JsonConvert.DeserializeObject<ServiceDepartmentItem>(json.Result);
                });
                task.Wait();
            }
            return item;
        }

        private List<ServiceDepartmentItem> GetServiceDepartmentList()
        {
            List<ServiceDepartmentItem> items;
            if (!_cache.TryGetValue("serviceDepartments", out items))
            {
                var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("GetServiceDepartments");
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<ServiceDepartmentItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    _cache?.Set("serviceDepartments", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }

        #endregion
    }
}
