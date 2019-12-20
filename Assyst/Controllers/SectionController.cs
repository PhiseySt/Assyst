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

        public IActionResult Sections(string name)
        {
            ViewData["name"] = name;
            return View(new List<SectionItem>());
        }

        public IActionResult SectionsPartialGrid(string name)
        {
            List<SectionItem> items = null;
            try
            {
                items = GetSectionList(name);
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

        public string GetJsonSectionById(long id)
        {
            return JsonConvert.SerializeObject(GetSectionById(id));
        }

        private SectionItem GetSectionById(long id)
        {
            SectionItem item = null;
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetSectionById"), id);
            var client = InitHttpClient();
            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                item = JsonConvert.DeserializeObject<SectionItem>(json.Result);
            });
            task.Wait();
            return item;
        }

        public string GetNoToNameSections(long value)
        {
            string name = null;
            var section = GetSectionById(value);
            if (section != null)
                name = section.name;
            return name;
        }

        private List<SectionItem> GetSectionList(string name)
        {
            name = name?.Trim();

            List<SectionItem> items = new List<SectionItem>();

            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(name)) queryParams.Add("name[like]", name);
            var serviceUrl = QueryHelpers.AddQueryString(AppConfig.HostUrl + AppConfig.GetUrlLink("GetSections"), queryParams);

            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                items = JsonConvert.DeserializeObject<List<SectionItem>>(json.Result);
            });
            task.Wait();

            return items;
        }

        #endregion
    }
}
