
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
        #region ActionResults

        public IActionResult ProductClasses(string name)
        {
            ViewData["name"] = name;
            return View(new List<ProductClassItem>());
        }

        public IActionResult ProductClassesPartialGrid(string shortCode, string name)
        {
            List<ProductClassItem> items = null;
            try
            {
                items = GetProductClassList(shortCode, name);
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

        private List<ProductClassItem> GetProductClassList(string shortCode, string name)
        {
            shortCode = shortCode?.Trim();
            name = name?.Trim();

            List<ProductClassItem> items = new List<ProductClassItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shortCode)) queryParams.Add("shortCode", shortCode);
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", "%" + name + "%");
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetProductClasses"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<ProductClassItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        public string GetNoToNameProductClasses(long value)
        {
            string name = null;
            var item = GetProductClassById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private ProductClassItem GetProductClassById(long id)
        {
            ProductClassItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetProductClassById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<ProductClassItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        #endregion
    }
}
