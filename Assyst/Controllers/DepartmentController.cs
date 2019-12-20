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

        public IActionResult Departments(string name)
        {
            ViewData["name"] = name;
            return View(new List<DepartmentItem>());
        }

        public IActionResult DepartmentsPartialGrid(string name)
        {
            List<DepartmentItem> items = null;
            try
            {
                items = GetDepartmentList(name);
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

        public string GetJsonDepartmentById(long id)
        {
            return JsonConvert.SerializeObject(GetDepartmentById(id));
        }

        private DepartmentItem GetDepartmentById(long id)
        {
            DepartmentItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetDepartmentById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<DepartmentItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameDepartments(long value)
        {
            string name = null;
            var item = GetDepartmentById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<DepartmentItem> GetDepartmentList(string name)
        {
            name = name?.Trim();

            List<DepartmentItem> items = new List<DepartmentItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", "%" + name + "%");
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetDepartments"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<DepartmentItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
