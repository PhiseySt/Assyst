using System;
using System.Collections.Generic;

namespace Assyst
{
    public static class AppConfig
    {
        public static string AuthorizationLogin { get; set; }
        public static string AuthorizationPassword { get; set; }
        // Время кэша в минутах для часто обновляемых справочников
        public static TimeSpan CacheStorageTime { get; set; }
        // Время кэша в минутах для редко обновляемых справочников
        public static TimeSpan LongCacheStorageTime { get; set; }
        // Время кэша в минутах для редко обновляемых справочников
        public static TimeSpan HttpWaitResponceTime { get; set; }
        // Время через которое осуществляется синхронизация с Assyst
        public static long AssystSynchronizationTime { get; set; }
        public static string HostUrl { get; set; }
        //максимальное количество записей, которое assyst позволяет вытащить за один restapi запрос
        public static int MaxCountRecords { get; set; }
        public static List<UrlManager> ListUrl { get; set; }
        // Разрешение на логирование в файл
        public static bool LogToFile { get; set; }
        public static void LoadConfig(string settingAuthorizationLogin, string settingAuthorizationPassword,  long cacheStorageTime, long longCacheStorageTime, long httpWaitResponceTime, long httpAssystSynchronizationTime)
        {
            CacheStorageTime = TimeSpan.FromMinutes(cacheStorageTime);
            LongCacheStorageTime= TimeSpan.FromMinutes(longCacheStorageTime);
            HttpWaitResponceTime= TimeSpan.FromMinutes(httpWaitResponceTime);
            AssystSynchronizationTime= httpAssystSynchronizationTime;
        }

        public static string GetUrlLink(string urlName)
        {
            foreach (var url in ListUrl)
            {
                if (url.UrlName == urlName) return url.UrlLink;
            }

            return "";
        }
    }
    /// <summary>
    /// Класс url
    /// </summary>
    public class UrlManager
    {
        /// <summary>Имя ссылки</summary>
        public string UrlName { get; set; }
        /// <summary>Url для запроса к Assyst Rest Api</summary>
        public string UrlLink { get; set; }
    }
}
