using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Assyst.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region Properties & Methods
        protected static List<ActionTypeItem> ListActionType { get; set; }

        private void InitActionType() => ListActionType = GetActionTypeList();


        private List<ActionTypeItem> GetActionTypeList()
        {
            List<ActionTypeItem> items;
            if (!_cache.TryGetValue("actiontypes", out items))
            {
                var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("ActionTypes");
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<ActionTypeItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    _cache?.Set("actiontypes", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }
        #endregion
    }
}
