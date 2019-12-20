using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Assyst.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Timer = System.Threading.Timer;


namespace Assyst.Controllers
{

    public partial class EventController
    {
        #region Properties & Methods


        // поле для хранения кэша
        private static IMemoryCache _cache;

        // поле для хранения Осталось времени последнего обновления кэша
        public static DateTime CacheTimeLastSynchStart = DateTime.Now;

        // поле для хранения Осталось времени последнего обновления кэша
        public static int CountSynchTread = 0;

        Timer _timerSynchAssyst ;

        public void StartSynch() => SynchSheduler();
        public void SynchSheduler()
        {
            var obj = 0;
            var firstTimeSyhch=AppConfig.AssystSynchronizationTime;
            var periodSyhch = AppConfig.AssystSynchronizationTime;
            TimerCallback tm = SynchCacheEvents;
            _timerSynchAssyst = new Timer(tm, obj, firstTimeSyhch, periodSyhch);
        }

        public void SynchCacheEvents(object obj)
        {
            var timeStartRequest = DateTime.Now;
            var periodSyhch = AppConfig.AssystSynchronizationTime;
            var timeDif = timeStartRequest - CacheTimeLastSynchStart;
            var timeDifms = (long) timeDif.TotalMilliseconds;
            // проверяем что с последней синхронизации уже прошло заданное количество Осталось времени (на данный момент синхронизация запускается по таймеру,
            // а также может запускаться пользователем через нажатие соответсвующей кнопки)
            if (timeDifms > periodSyhch)
            {
                List<EventItem> events = null;
                {
                    var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("GetEvents");
                    var client = InitHttpClient();
                    var task = client.GetAsync(serviceUrl).ContinueWith(requestTask =>
                    {
                        var resultMessage = GetExceptionMessage(requestTask);
                        if (resultMessage != "success")
                            throw new HttpRequestException(resultMessage);

                        var response = requestTask.Result;
                        var json = response.Content.ReadAsStringAsync();
                        json.Wait(AppConfig.HttpWaitResponceTime);
                        events = JsonConvert.DeserializeObject<List<EventItem>>(json.Result);
                    });
                    task.Wait(AppConfig.HttpWaitResponceTime);

                    InitCache();
                    _cache?.Set("events", events,
                           new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.CacheStorageTime));
                    CacheTimeLastSynchStart = timeStartRequest;
                }
            }
        }

        private void InitCache()
        {
            InitCategoryData();
            InitPrioritityData();
            InitSeriousnessData();
            InitActionType();
            InitServiceDepartmentData();
        }

        #endregion

        #region Cache

        public const string eventCacheKey = "events";

        public EventItem GetCacheEvent(long id)
        {
            List<EventItem> items;
            if (_cache.TryGetValue(eventCacheKey, out items))
            {
                return items.FirstOrDefault(i => i.id == id);
            }
            return null;
        }

        public void SetCacheEvents(List<EventItem> eventItems)
        {
            List<EventItem> items;
            if (!_cache.TryGetValue(eventCacheKey, out items))
            {
                items.AddRange(eventItems);
            }
            else
            {
                foreach (var ev in eventItems)
                {
                    var index = items.FindIndex(r => r.id == ev.id);
                    if (index != -1)
                    {
                        items[index] = ev;
                    }
                    else
                        items.Add(ev);
                }
            }
            _cache?.Set(
                eventCacheKey,
                items,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
        }

        public void SetCacheEvent(EventItem eventItem)
        {
            if (eventItem.eventStatus != 1 || eventItem.assignedServDeptId != 260)
                return;
            List<EventItem> items;
            if (_cache.Get(eventCacheKey) == null)
                return;
            if (!_cache.TryGetValue(eventCacheKey, out items))
            {
                items.Add(eventItem);
            }
            else
            {
                var index = items.FindIndex(r => r.id == eventItem.id);
                if (index != -1)
                {
                    items[index] = eventItem;
                }
                else
                    items.Add(eventItem);
            }
            _cache?.Set(
                eventCacheKey,
                items,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
        }

        public void DeleteCacheEvent(long id)
        {
            List<EventItem> items;
            if (!_cache.TryGetValue(eventCacheKey, out items)) return;
            var item = items.FirstOrDefault(r => r.id == id);
            if (item == null) return;
            items.Remove(item);
            _cache?.Set(
                eventCacheKey,
                items,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
        }

        public void RemoveCacheDepartment()
        {
            _cache.Remove(eventCacheKey);
        }

        #endregion

    }
}