using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Assyst.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Serilog;

namespace Assyst.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        public IActionResult LoginError()
        {
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var listCorrectUsers = GetInfoUser(model.Login, model.Password);
                    var authenticateUser = new AssystUserItem();
                    if (listCorrectUsers != null) authenticateUser = listCorrectUsers.FirstOrDefault();
                    if (listCorrectUsers != null && authenticateUser != null)
                    {
                        await Authenticate(model.Login + ":" + model.Password); // аутентификация
                        return RedirectToAction("Monitor", "Event");
                    }

                    throw new HttpRequestException("Неправильный логин или пароль");
                }
                catch (Exception e)
                {
                    TempData["errorMessage"] = e.InnerException?.Message ?? e.Message;
                    return RedirectToAction("LoginError", "Account");
                }
            }

            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private HttpClient InitAuthenticationHttpClient(string login, string password)
        {
            var client = new HttpClient { Timeout = AppConfig.HttpWaitResponceTime };
            var autorizationData = login + ":" + password;
            var tokenAuthorization = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(autorizationData));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", tokenAuthorization);
            return client;
        }

        private List<AssystUserItem> GetInfoUser(string login, string password)
        {
            var client = InitAuthenticationHttpClient(login, password);
            var serviceUrl = AppConfig.HostUrl + string.Format(AppConfig.GetUrlLink("GetAssystUserByShortCode"), login);
            var listCorrectUsers = new List<AssystUserItem>();
            var task = client.GetAsync(serviceUrl).ContinueWith(requestTask =>
            {
                var response = requestTask.Result;
                var json = response.Content.ReadAsStringAsync();
                var resultMessage = GetExceptionMessage(requestTask);
                if (resultMessage != "success")
                    throw new HttpRequestException(resultMessage);

                json.Wait();
                listCorrectUsers = JsonConvert.DeserializeObject<List<AssystUserItem>>(json.Result);
            });
            task.Wait();
            return listCorrectUsers;
        }

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
                    if (exception != null && exception.type != null)
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
    }
}