using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assyst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Assyst.Controllers
{
    public class HttpClientServise
    {
        public HttpClient InitHttpClient(string autorizationData)
        {
            var client = new HttpClient();
            var tokenAuthorization = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(autorizationData));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", tokenAuthorization);
            return client;
        }
    }

}