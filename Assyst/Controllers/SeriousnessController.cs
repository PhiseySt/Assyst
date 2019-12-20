
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
        #region ActionResults

        #endregion

        #region Properties & Methods

        protected List<SeriousnessItem> ListSeriousness { get; set; }

        private void InitSeriousnessData() => ListSeriousness = GetSeriousnessList();

        private List<SeriousnessItem> GetSeriousnessList()
        {
            List<SeriousnessItem> items;
            if (!_cache.TryGetValue("seriousnesses", out items))
            {
                var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("GetSeriousnesses");
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<SeriousnessItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    _cache?.Set("seriousnesses", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }

        #endregion
    }
}
