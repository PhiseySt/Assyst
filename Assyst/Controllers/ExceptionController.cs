using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Assyst.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Assyst.Controllers
{
    public partial class EventController
    {
        // Примеры ошибок возвращаемых системой assist:

        // при сохранении заявки с неуказанным заявителем (пользователем)
        // {"type":"ComplexValidationException","message":"A complex validation error has been detected by the application.","messageKey":"rest.exceptions.ComplexValidationException",
        // "errors":[{"rule":"fileupload.typenotallowed","field":"file","messageKey":"server.validation.errors.fileupload.typenotallowed","message":
        // "The file type is not supported for this operation.","failingObjectName":"pngbmp.bmp"}]}

        // при сохранении вложения в неподдержиаемом формате картинки bmp
        // {"type":"ComplexValidationException","message":"A complex validation error has been detected by the application.","messageKey":"rest.exceptions.ComplexValidationException",
        //"errors":[{"rule":"mandatoryAffectedUser","field":"affectedUser","messageKey":"server.validation.errors.mandatoryAffectedUser","message":
        //"Affected User is required.","diagnostic":"","objectFailingValidationClass":"com.axiossystems.assyst.dto.events.EventDto"}]}

        // Для случая запроса более 1000 записей блок errors в json не возвращается
        //{"type":"TooManyRowsException","message":"The requested search would return more data than is allowed. Please refine the search criteria.","messageKey":"rest.exceptions.TooManyRowsException",
        // "diagnostic":"More than 1000 rows retrieved"}

        private static string GetExceptionMessage(Task<HttpResponseMessage> requestTask)
        {
            string exceptionMessage = "success";
            if (requestTask.IsFaulted)
            {
                // faulted with exception
                Exception ex = requestTask.Exception;
                while (ex is AggregateException && ex.InnerException != null)
                    ex = ex.InnerException;
                exceptionMessage = ex.Message;
            }
            else if (requestTask.IsCanceled)
            {
                // this should not happen 
                // as you don't pass a CancellationToken into your task
                exceptionMessage = "Cancelled";
            }
            else
            {
                try
                {
                    var json = requestTask.Result.Content.ReadAsStringAsync();
                    var exception = JsonConvert.DeserializeObject<ExceptionItem>(json.Result);
                    if(exception != null && exception.type != null)
                        exceptionMessage = exception.htmlText;
                }
                catch
                {
                    return exceptionMessage;
                }
            }
            if (AppConfig.LogToFile) Log.Information("Error" + " " + exceptionMessage);
            if (exceptionMessage == "success")
                return exceptionMessage;
            return "***" + exceptionMessage + "***";
        }

        private const string Delimiter = "***";

        private static string GetShortNameError(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "неизвестная ошибка";

            dynamic outerDynamicJson = JObject.Parse(message);
            var outerExceptionMessage = outerDynamicJson.message;
            var result = "";
            try
            {
                var innerDynamicJson = outerDynamicJson.errors[0].ToString();
                int indexOfMessage = innerDynamicJson.IndexOf("\"message\"");
                int indexOfСomma = innerDynamicJson.IndexOf("\",", indexOfMessage);
                var innerExceptionMessage = innerDynamicJson.Substring(indexOfMessage, indexOfСomma - indexOfMessage).Replace("\"", "");
                result = Delimiter + outerExceptionMessage + " : " + innerExceptionMessage + Delimiter;
            }
            catch
            {
                return Delimiter + outerExceptionMessage + Delimiter;
            }

            return result;
        }
    }
}
