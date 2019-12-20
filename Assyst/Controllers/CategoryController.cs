using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Assyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region ActionResults

        public IActionResult CategoriesPartialCb(long? relatedItemId, long? eventCategoryId)
        {
            if (relatedItemId == null)
                return PartialView(null);
            var addCategories = new List<long>(22);
            if (eventCategoryId != null)
                addCategories.Add((long)eventCategoryId);
            return PartialView(GetCategoryList((long)relatedItemId, addCategories));
        }

        public IActionResult Categories()
        {
            return View(new List<CategoryItem>());
        }

        public IActionResult CategoriesPartialGrid(string shortCode, string name)
        {
            return PartialView(GetCategoryList(shortCode, name));
        }

        #endregion

        #region Properties & Methods
        protected List<CategoryItem> ListCategory { get; set; }

        private void InitCategoryData() => ListCategory = GetCategoryList();

        public string GetJsonCategoryById(long id)
        {
            return JsonConvert.SerializeObject(GetCategoryById(id));
        }

        private CategoryItem GetCategoryById(long id)
        {
            var item = GetCategoryList().FirstOrDefault(e => e.id == id);
            if (item == null)
            {
                var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetCategoryById"), id);
                var client = InitHttpClient();
                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    item = JsonConvert.DeserializeObject<CategoryItem>(json.Result);
                });
                task.Wait();
            }
            return item;
        }

        public string GetNoToNameCategories(long value)
        {
            string name = null;
            var category = GetCategoryById(value);
            if (category != null)
                name = category.name;
            return name;
        }

        private List<CategoryItem> GetCategoryList()
        {
            List<CategoryItem> items;
            if (!_cache.TryGetValue("categories", out items))
            {
                var serviceUrl = AppConfig.HostUrl + AppConfig.GetUrlLink("GetCategories");
                var client = InitHttpClient();

                var task = client.GetAsync(serviceUrl).ContinueWith((requestTask) =>
                {
                    var resultMessage = GetExceptionMessage(requestTask);
                    if (resultMessage != "success")
                        throw new HttpRequestException(resultMessage);

                    var response = requestTask.Result;
                    var json = response.Content.ReadAsStringAsync();
                    json.Wait();
                    items = JsonConvert.DeserializeObject<List<CategoryItem>>(json.Result);
                });
                task.Wait();
                if (items.Any())
                {
                    _cache?.Set("categories", items,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(AppConfig.LongCacheStorageTime));
                }
            }
            return items;
        }

        private List<CategoryItem> GetCategoryList(string shortCode, string name)
        {
            var categoryList = GetCategoryList();
            if (!string.IsNullOrEmpty(name))
                categoryList = categoryList.Where(p => p.name.Contains(name)).ToList();
            if (!string.IsNullOrEmpty(shortCode))
                categoryList = categoryList.Where(p => p.shortCode == shortCode).ToList();
            return categoryList;
        }

        private List<CategoryItem> GetCategoryList(long itemId, List<long> addCategories)
        {
            var eventBuilderList = GetEventBuilderList(itemId);
            if (eventBuilderList == null || eventBuilderList.Count == 0)
                return GetCategoryList().Where(r => addCategories.Contains(r.id)).ToList();
            return GetCategoryList().Where(p => eventBuilderList.Select(t => t.categoryId).Contains(p.id) || addCategories.Contains(p.id)).OrderBy(s => s.name).Distinct().ToList();
        }

        public List<CategoryItem> GetCauseCategoryList()
        {
            return new List<CategoryItem>()
            {
                new CategoryItem {id = 741, name = "_CRM_CLM_НЕОБУЧ ПОЛЬЗ_ОБРАБОТКА ИНЦИДЕНТА"},
                new CategoryItem {id = 742, name = "_CRM_OMS_БЛОКИРОВКА/ВОССТАНОВЛЕНИЕ ПРОМО-КОДА"},
                new CategoryItem {id = 509, name = "_ВЕДЕНИЕ МАСТЕР ДАННЫХ"},
                new CategoryItem {id = 499, name = "_ДУБЛЬ"},
                new CategoryItem {id = 538, name = "_ЕДИНИЧНАЯ ВЫГРУЗКА ДАННЫХ"},
                new CategoryItem {id = 528, name = "_НАСТРОЙКА ПРАВ ДОСТУПА"},
                new CategoryItem {id = 518, name = "_НЕ ПО АДРЕСУ"},
                new CategoryItem {id = 495, name = "_НЕКОРРЕКТНЫЕ МАСТЕР-ДАННЫЕ (ОШИБКА БИЗНЕСА)"},
                new CategoryItem {id = 497, name = "_НЕОБУЧЕННОСТЬ ПОЛЬЗОВАТЕЛЯ"},
                new CategoryItem {id = 519, name = "_НЕОБУЧЕНОСТЬ ПОЛЬЗОВАТЕЛЯ ОР"},
                new CategoryItem {id = 481, name = "_НЕПРАВИЛЬНАЯ КОНФИГУРАЦИЯ FRONT OFFICE"},
                new CategoryItem {id = 498, name = "_НЕПРАВИЛЬНОЕ ИСПОЛЬЗОВАНИЕ СИСТЕМЫ"},
                new CategoryItem {id = 496, name = "_ОБНОВЛЕНИЕ ВЕРСИИ"},
                new CategoryItem {id = 241, name = "_ОБРЫВ СВЯЗИ"},
                new CategoryItem {id = 541, name = "_ОБРЫВ СВЯЗИ В ЧАТЕ"},
                new CategoryItem {id = 493, name = "_ОШИБКА ATG"},
                new CategoryItem {id = 485, name = "_ОШИБКА CIS INETSHOP"},
                new CategoryItem {id = 486, name = "_ОШИБКА CIS МОБККМ"},
                new CategoryItem {id = 487, name = "_ОШИБКА MS SHAREPOINT"},
                new CategoryItem {id = 484, name = "_ОШИБКА POS DM"},
                new CategoryItem {id = 492, name = "_ОШИБКА SAP CRM"},
                new CategoryItem {id = 485, name = "_ОШИБКА SAP DS"},
                new CategoryItem {id = 490, name = "_ОШИБКА SAP ERP"},
                new CategoryItem {id = 488, name = "_ОШИБКА SAP HR"},
                new CategoryItem {id = 537, name = "_ОШИБКА SAP TM БТЕ"},
                new CategoryItem {id = 530, name = "_ОШИБКА SP"},
                new CategoryItem {id = 494, name = "_ОШИБКА В ИНТЕГРАЦИИ"},
                new CategoryItem {id = 526, name = "_ОШИБКА В ОТЧЕТЕ"},
                new CategoryItem {id = 527, name = "_ОШИБКА В ПРОЦЕДУРЕ ЗАГРУЗКИ"},
                new CategoryItem {id = 547, name = "_ОШИБКА ДИЗАЙНА"},
                new CategoryItem {id = 507, name = "_ОШИБКА НАСТРОЕК РМ"},
                new CategoryItem {id = 378, name = "_ОШИБКА ПО NTSWINCASH (ПРИЧИНА ЗАКРЫТИЯ ДЛЯ ТРЕТЬИХ СИСТЕМ)"},
                new CategoryItem {id = 517, name = "_РЕГЛАМЕНТ/АКЦИЯ"},
                new CategoryItem {id = 524, name = "_СБОЙ ЗАГРУЗКИ BW"},
                new CategoryItem {id = 525, name = "_СБОЙ ЗАГРУЗКИ IQ"},
                new CategoryItem {id = 522, name = "_СИСТЕМНЫЙ СБОЙ BO"},
                new CategoryItem {id = 523, name = "_СИСТЕМНЫЙ СБОЙ BW/HANA"},
                new CategoryItem {id = 521, name = "_СИСТЕМНЫЙ СБОЙ IQ"},
                new CategoryItem {id = 506, name = "_ТЕСТИРОВАНИЕ"},
                new CategoryItem {id = 758, name = "BI_ИЗМЕНЕНИЕ МАСТЕР-ДАННЫХ"},
                new CategoryItem {id = 759, name = "BI_КОНСУЛЬТАЦИЯ ПОЛЬЗОВАТЕЛЯ"},
                new CategoryItem {id = 760, name = "BI_НЕИЗВЕСТНАЯ ПРИЧИНА"},
                new CategoryItem {id = 761, name = "BI_НЕКОРРЕКТНО СОЗДАННАЯ ЗАЯВКА"},
                new CategoryItem {id = 763, name = "BI_ПЕРЕНОС ОТЧЕТОВ"},
                new CategoryItem {id = 764, name = "BI_ПЕРЕПРИВЯЗКА ЧЕКОВ"},
                new CategoryItem {id = 765, name = "BI_ПРОБЛЕМА НА СТОРОНЕ ИСХОДНОЙ СИСТЕМЫ"},
                new CategoryItem {id = 766, name = "BI_ПРОБЛЕМЫ С ДОСТУПОМ В СИСТЕМУ"},
                new CategoryItem {id = 767, name = "BI_ПРОБЛЕМЫ С ПРОИЗВОДИТЕЛЬНОСТЬЮ СИСТЕМЫ"},
                new CategoryItem {id = 760, name = "BI_ПУБЛИКАЦИИ. СБОЙ ПРИ РАССЫЛКЕ/ВЫГРУЗКЕ"},
                new CategoryItem {id = 770, name = "BI_ПУБЛИКАЦИИ. СОЗДАНИЕ НОВЫХ РАССЫЛОК/ВЫГРУЗОК"},
                new CategoryItem {id = 771, name = "BI_СБОЙ ПРИ ЗАГРУЗКЕ ДАННЫХ"},
                new CategoryItem {id = 735, name = "CRM_CLM_ОШИБКА SAP CRM_'ВВЕДЕНО СЛИШКОМ МНОГО….(ГРУППА ОБРАБОТКИ)'"},
                new CategoryItem {id = 726, name = "CRM_CLM_ОШИБКА SAP CRM_ДЕЛОВОЙ ПАРТНЕР *** ИМЕЕТ СТАТУС \"БЛОКИРОВАНО\""},
                new CategoryItem {id = 725, name = "CRM_CLM_ОШИБКА SAP HR_НЕ ОТПРАВЛЕНЫ АКТУАЛЬНЫЕ ДАННЫЕ"},
                new CategoryItem {id = 729, name = "CRM_CLM_ТЕСТИРОВАНИЕ"},
                new CategoryItem {id = 626, name = "CRM_LOY_КОРРЕКТИРОВКА ДАННЫХ КЛИЕНТА"},
                new CategoryItem {id = 724, name = "CRM_LOY_НЕИЗВЕСТНАЯ ПРИЧИНА"},
                new CategoryItem {id = 719, name = "CRM_LOY_НЕОБУЧ. ПОЛЬЗ._ПРАВИЛА ЛОЯЛЬНОСТИ"},
                new CategoryItem {id = 721, name = "CRM_LOY_НЕТ ЗАКРЫВАЮЩЕГО СТАТУСА ПО ЗАКАЗУ"},
                new CategoryItem {id = 717, name = "CRM_LOY_ОШИБКА ATG_РЕЗЕРВ ПОД НЕСУЩЕСТВУЮЩИЙ ЗАКАЗ"},
                new CategoryItem {id = 737, name = "CRM_LOY_ОШИБКА CRM_РУЧНОЕ ОБЪЕДИНЕНИЕ БК"},
                new CategoryItem {id = 718, name = "CRM_LOY_ОШИБКА NTS_СТОРНО ЧЕКА"},
                new CategoryItem {id = 739, name = "CRM_LOY_ОШИБКА POS DM_НЕ ПРОГРУЖЕН ЗАКРЫВАЮЩИЙ ЧЕК В CRM"},
                new CategoryItem {id = 716, name = "CRM_LOY_ОШИБКА SAP CRM_НЕ ОБРАБОТАНО ДУ"},
                new CategoryItem {id = 722, name = "CRM_LOY_СБОЙ НА СТОРОНЕ ПРОВАЙДЕРА"},
                new CategoryItem {id = 723, name = "CRM_LOY_ТЕСТИРОВАНИЕ"},
                new CategoryItem {id = 711, name = "CRM_OMS_\"НЕ ВЫДАНА\"_НАРУШЕНИЕ ДОСТАВКИ"},
                new CategoryItem {id = 752, name = "CRM_OMS_ЗАКАЗ НЕ ФИНАЛИЗИРОВАН_CRM"},
                new CategoryItem {id = 754, name = "CRM_OMS_ЗАКАЗ НЕ ФИНАЛИЗИРОВАН_ERP-OMS"},
                new CategoryItem {id = 751, name = "CRM_OMS_ЗАКАЗ НЕ ФИНАЛИЗИРОВАН_ТМ"},
                new CategoryItem {id = 710, name = "CRM_OMS_КОРРЕКТИРОВКА ДАННЫХ СОЗДАНИЕ СВЯЗАННОГО"},
                new CategoryItem {id = 714, name = "CRM_OMS_НЕИЗВЕСТНАЯ ПРИЧИНА"},
                new CategoryItem {id = 705, name = "CRM_OMS_НЕОБУЧ. ПОЛЬЗ._НЕТ ОСТАТКОВ МАТЕРИАЛА"},
                new CategoryItem {id = 706, name = "CRM_OMS_НЕОБУЧ. ПОЛЬЗ._ПРОЦЕСС РЕЗЕРВА \\ СКЛАДСКАЯ ЛОГИСТИКА"},
                new CategoryItem {id = 704, name = "CRM_OMS_НЕОБУЧ. ПОЛЬЗ._СТАНДАРТНАЯ ОШИБКА СИСТЕМЫ"},
                new CategoryItem {id = 749, name = "CRM_OMS_ОШИБКА ATG_НЕКОРРЕКТНАЯ СУММА/СКИДКА ПО КРЕДИТУ"},
                new CategoryItem {id = 698, name = "CRM_OMS_ОШИБКА ATG_НЕКОРРЕТНЫЕ ДАННЫЕ ПО СУММЕ ЗАКАЗА"},
                new CategoryItem {id = 746, name = "CRM_OMS_ОШИБКА ATG_ОТМЕНА РЕЗЕРВА БР В АКТУАЛЬНОМ ЗАКАЗЕ"},
                new CategoryItem {id = 748, name = "CRM_OMS_ОШИБКА ATG_ОТСУТСТВУЕТ ЛОГ 060_02"},
                new CategoryItem {id = 696, name = "CRM_OMS_ОШИБКА ATG_РЕЗЕРВ ПОД НЕСУЩЕСТВУЮЩИЙ ЗАКАЗ"},
                new CategoryItem {id = 699, name = "CRM_OMS_ОШИБКА ERP_ НЕ ПРИШЕЛ ЛИД ТАЙМ"},
                new CategoryItem {id = 702, name = "CRM_OMS_ОШИБКА NTS \"THE RESERVATION COULD NOT BE CREATED\""},
                new CategoryItem {id = 747, name = "CRM_OMS_ОШИБКА NTS_\"NTS.WINCASH.WEBSERVICE.CUSTOMER\"</"},
                new CategoryItem {id = 700, name = "CRM_OMS_ОШИБКА NTS_\"THE DISPATCHING OPERATION TO THE SHOP\""},
                new CategoryItem {id = 750, name = "CRM_OMS_ОШИБКА NTS_«UPDATING OR DELETING FAILED»"},
                new CategoryItem {id = 692, name = "CRM_OMS_ОШИБКА SAP CRM_МНОГО ПОСТАВЩИКОВ"},
                new CategoryItem {id = 691, name = "CRM_OMS_ОШИБКА SAP CRM_ОПЕРАЦИЯ УЖЕ ОБРАБАТЫВАЕТСЯ ПОЛЬЗОВАТЕЛЕМ"},
                new CategoryItem {id = 694, name = "CRM_OMS_ОШИБКА SAP CRM_РАСЧЕТ СТОИМОСТИ ЗАКАЗА"},
                new CategoryItem {id = 695, name = "CRM_OMS_ОШИБКА СП_РАСЧЕТ СТОИМОСТИ"},
                new CategoryItem {id = 712, name = "CRM_OMS_ПРОДУКТ ОТСУТСТВУЕТ \\ ЧАСТЬ ДАННЫХ ОШИБОЧНА"},
                new CategoryItem {id = 654, name = "CRM_ВЕДЕНИЕ НАСТРОЕК \\ МАСТЕР ДАННЫХ"},
                new CategoryItem {id = 658, name = "CRM_ВЫГРУЗКА ДАННЫХ"},
                new CategoryItem {id = 731, name = "CRM_ДАМП\\ERROR\\БАГ"},
                new CategoryItem {id = 652, name = "CRM_ДУБЛЬ"},
                new CategoryItem {id = 649, name = "CRM_НЕ ПО АДРЕСУ"},
                new CategoryItem {id = 732, name = "CRM_ОШИБКА SAP PI_ ОШИБКА ПЕРЕДАЧИ ДАННЫХ"},
                new CategoryItem {id = 653, name = "CRM_ОШИБКА МАСТЕР-ДАННЫХ"},
                new CategoryItem {id = 733, name = "CRM_РОЛИ\\ПОЛНОМОЧИЯ (ЗАКРЫТЬ С ПРОСЬБОЙ СОЗДАТЬ ЗАЯВКУ ПО ФОРМЕ)"},
                new CategoryItem {id = 689, name = "EWM_ЗАПРОС ВЫПОЛНЕН"},
                new CategoryItem {id = 687, name = "EWM_ИНЦИДЕНТ РЕШЁН"},
                new CategoryItem {id = 688, name = "EWM_КОНСУЛЬТАЦИЯ ПОЛЬЗОВАТЕЛЯ"},
                new CategoryItem {id = 690, name = "EWM_НЕКОРРЕКТНОЕ ЗАВЕДЕНИЕ"},
                new CategoryItem {id = 557, name = "HR_ДОП. ТРЕБОВАНИЯ (БУДЕТ ЗАРЕГИСТРИРОВАН RFC)"},
                new CategoryItem {id = 582, name = "HR_ДУБЛЬ"},
                new CategoryItem {id = 485, name = "HR_НЕИЗВЕСТНАЯ ПРИЧИНА(ОШИБКА НЕ ВОСПРОИЗВОДИТСЯ)"},
                new CategoryItem {id = 562, name = "HR_НЕКОРРЕКТНЫЕ МАСТЕР-ДАННЫЕ (ОШИБКА БИЗНЕСА)"},
                new CategoryItem {id = 555, name = "HR_ОШИБКА ПОЛЬЗОВАТЕЛЯ (ЕСТЬ ИНСТРУКЦИЯ ПО ПРОЦЕССУ)"},
                new CategoryItem {id = 558, name = "HR_ОШИБКА СТАНДАРТА ПО (ВЫСТАВЛЕН MESSAGE В SAP)"},
                new CategoryItem {id = 579, name = "HR_РОЛИ И ПОЛНОМОЧИЯ"},
                new CategoryItem {id = 669, name = "SAPFOBO_ИЗМЕНЕНИЕ И ОТПРАВКА ДАННЫХ"},
                new CategoryItem {id = 670, name = "SAPFOBO_НЕЗВЕСТНАЯ ПРИЧИНА (ЕДИНИЧНЫЙ БАГ)"},
                new CategoryItem {id = 671, name = "SAPFOBO_НЕКОРРЕКТНЫЕ МАСТЕРДАННЫЕ"},
                new CategoryItem {id = 672, name = "SAPFOBO_НЕТ ТОВАРА ПОД РЕЗЕРВ"},
                new CategoryItem {id = 674, name = "SAPFOBO_ОШИБКА НЕКОРРЕКТНОЙ ВЫГРУЗКИ КУПОНА УЦЕНКИ"},
                new CategoryItem {id = 675, name = "SAPFOBO_ОШИБКА ОБРАБОТКИ ЧЕКА"},
                new CategoryItem {id = 676, name = "SAPFOBO_ОШИБКА ПО ЧЕКАМ FOBO"},
                new CategoryItem {id = 677, name = "SAPFOBO_ОШИБКА ПРИ СОЗДАНИИ ЧЕКА CRM"},
                new CategoryItem {id = 678, name = "SAPFOBO_ОШИБКА ПРИ СОЗДАНИИ ЧЕКА FOBO"},
                new CategoryItem {id = 680, name = "SAPFOBO_ОШИБКИ С ЗНД"},
                new CategoryItem {id = 681, name = "SAPFOBO_ПОВТОРНАЯ ВЫГРУЗКА ЧЕКА"},
                new CategoryItem {id = 683, name = "SAPFOBO_РЕГЛАМЕНТНЫЕ РАБОТЫ"},
                new CategoryItem {id = 684, name = "SAPFOBO_ЧЕК ВЫГРУЖЕН ПОЗЖЕ ДАТЫ ПРОВОДКИ"},
                new CategoryItem {id = 685, name = "SAPFOBO_ЧЕК НЕ РЕЛЕВАНТЕН ДЛЯ ВЫГРУЗКИ В CRM"},
                new CategoryItem {id = 564, name = "TM_КОНСУЛЬТАЦИЯ ПОЛЬЗОВАТЕЛЯ"},
                new CategoryItem {id = 575, name = "TM_ОШИБКА В РАСЧЁТЕ ЗАТРАТ (БИЗНЕС)"},
                new CategoryItem {id = 574, name = "TM_ОШИБКА В РАСЧЁТЕ ЗАТРАТ (ТЕХНИЧЕСКАЯ)"},
                new CategoryItem {id = 566, name = "TM_ОШИБКА КОНТРАГЕНТА (ОШИБКА БИЗНЕСА)"},
                new CategoryItem {id = 569, name = "TM_ОШИБКА НАСТРОЕК ИЛИ МАСТЕР ДАННЫХ (ОШИБКА БИЗНЕСА)"},
                new CategoryItem {id = 567, name = "TM_ОШИБКА ПЛАНИРОВАНИЯ (ОШИБКА БИЗНЕСА)"},
                new CategoryItem {id = 573, name = "TM_ОШИБКА СТАТУСОВ ВЫПОЛНЕНИЯ (БИЗНЕС)"},
                new CategoryItem {id = 572, name = "TM_ОШИБКА СТАТУСОВ ВЫПОЛНЕНИЯ (ТЕХНИЧЕСКАЯ)"},
                new CategoryItem {id = 577, name = "TM_ОШИБКИ ДРФ"},
                new CategoryItem {id = 578, name = "TM_ОШИБКИ КОНКУРСОВ"},
                new CategoryItem {id = 571, name = "TM_СИСТЕМНАЯ ОШИБКА SAP TM"},
                new CategoryItem {id = 27, name = "НЕИЗВЕСТНАЯ"},
                new CategoryItem {id = 502, name = "ОБЪЕКТ - СБОЙ ОБОРУДОВАНИЯ"},
                new CategoryItem {id = 503, name = "ОБЪЕКТ - СБОЙ ПИТАНИЯ"},
                new CategoryItem {id = 500, name = "ПРОБЛЕМА С АППАРАТНОЙ ЧАСТЬЮ"},
                new CategoryItem {id = 505, name = "ЦО - СБОЙ ОБОРУДОВАНИЯ"}
            };
        }

        #endregion
    }
}
