using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assyst.Models;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        #region Properties & Methods

        private List<LinkedEventGroupItem> GetLinkedEventGroups(long eventId)
        {
            List<LinkedEventGroupItem> linkedEventGroups = null;

            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetLinkedEventGroup"), eventId);
            var client = InitHttpClient();

            var task = client.GetAsync(serviceUrl).ContinueWith(requestTask =>
            {
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                json.Wait();
                linkedEventGroups = JsonConvert.DeserializeObject<List<LinkedEventGroupItem>>(json.Result);
            });
            task.Wait();

            return linkedEventGroups;
        }

        private Task<string> SaveLinkedEventGroup(long lnkReasonId)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                linkGroupRemarks = "Группа связанных заявок",
                linkReasonId = lnkReasonId
            });
            var content = jsonBody;
            var url = AppConfig.HostUrl + AppConfig.GetUrlLink("LinkedEventGroups");
            var serviceUrl = url;
            var request = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
            {
                Content = new StringContent(
                  content,
                  Encoding.UTF8,
                  "application/json"
              )
            };
            var contentsResponce = SendRequest(request);
            return contentsResponce;
        }

        #endregion
    }
}
