
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

        public IActionResult Products(string name)
        {
            ViewData["name"] = name;
            return View(new List<ProductItem>());
        }

        public IActionResult ProductsPartialGrid(string shortCode, string name, long? productClassId)
        {
            List<ProductItem> items = null;
            try
            {
                items = GetProductList(shortCode, name, productClassId);
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

        private ProductItem GetProductById(long id)
        {
            ProductItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetProductById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<ProductItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameProducts(long value)
        {
            string name = null;
            var item = GetProductById(value);
            if (item != null)
                name = item.name;
            return name;
        }

        private List<ProductItem> GetProductList(string shortCode, string name, long? productClassId)
        {
            shortCode = shortCode?.Trim();
            name = name?.Trim();

            List<ProductItem> items = new List<ProductItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(shortCode)) queryParams.Add("shortCode", shortCode);
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", "%" + name + "%");
            if (productClassId != null) queryParams.Add("productClassId", productClassId.ToString());
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
                items = JsonConvert.DeserializeObject<List<ProductItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
