using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
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

        #endregion

        #region Properties & Methods

        public string GetNoToNameEventBuilders(long value)
        {
            string name = null;
            var category = GetCategoryById(value);
            if (category != null)
                name = category.name;
            return name;
        }

        public string GetJsonEventBuilderById(long categoryId, long relatedItemId)
        {
            return JsonConvert.SerializeObject(GetEventBuilderByCategoryId(categoryId, relatedItemId));
        }

        public EventBuilderItem GetEventBuilderByCategoryId(long categoryId, long relatedItemId)
        {
            return GetEventBuilderList(relatedItemId).FirstOrDefault(eb => eb.categoryId == categoryId);
        }

        private List<EventBuilderItem> GetEventBuilderList(long itemId)
        {
            List<EventBuilderItem> items;
            if (!_cache.TryGetValue("eventBuilders["+ itemId + "]", out items))
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetEventBuildersByItemId"), itemId);
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<EventBuilderItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    SetCategoryFields(items);
                    _cache?.Set("eventBuilders[" + itemId + "]", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }

        private void SetCategoryFields(IEnumerable<EventBuilderItem> eventBuilers)
        {
            foreach (var eventBuiler in eventBuilers)
            {
                var category = GetCategoryList().FirstOrDefault(q => q.id == eventBuiler.categoryId);
                if (category != null)
                {
                    eventBuiler.categoryName = category.name;
                    eventBuiler.categoryShortCode = category.shortCode;
                }
            }
        }

        #endregion
    }
}
