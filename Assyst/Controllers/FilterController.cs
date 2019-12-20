using System;
using System.Collections.Generic;
using System.Linq;
using Assyst.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        public static string GetCacheFilterValByUserId(string userName)
        {
            var currentUserName = userName.Substring(0, userName.IndexOf(":", StringComparison.Ordinal));
            List<FilterModel> itemsFilterCache;
            if (!_cache.TryGetValue("filters", out itemsFilterCache))
            {
                itemsFilterCache = new List<FilterModel>();
            }
            var itemFilter = itemsFilterCache.FirstOrDefault(q => q.userName == currentUserName);
            var jsonBody = JsonConvert.SerializeObject(itemFilter);
            return jsonBody;
        }

        private void SetCacheFilterValByUserId(FilterModel itemFilterModel)
        {
            List<FilterModel> itemsFilterCache;
            if (!_cache.TryGetValue("filters", out itemsFilterCache))
            {
                itemsFilterCache = new List<FilterModel>();
            }
            var index = itemsFilterCache.FindIndex(r => r.userName == itemFilterModel.userName);
            if (index != -1)
            {
                itemsFilterCache[index] = itemFilterModel;
            }
            else
                itemsFilterCache.Add(itemFilterModel);
            _cache.Set("filters", itemsFilterCache,
                       new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.CacheStorageTime));
        }
    }
}