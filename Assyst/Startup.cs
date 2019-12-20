using System;
using System.Collections.Generic;
using System.Globalization;
using Assyst.Controllers;
using Assyst.Models;
using Assyst.Service;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Bootstrap;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Assyst
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // установка конфигурации подключения
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                    options.LoginPath = new PathString("/Account/Login"));

            services
               .AddDevExpressControls(settings =>
               {
                   settings.Resources = ResourcesType.ThirdParty | ResourcesType.DevExtreme;
                   settings.Bootstrap(bootstrapOptions => bootstrapOptions.IconSet = BootstrapIconSet.FontAwesome);
               })//use before AddMvc()
               .AddMvc()
               .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandler>();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            app.UseDevExpressControls();
            app.UseStaticFiles();
            app.UseAuthentication();
            InitAppConfig();
            app.UseMvc(routes => routes.MapRoute(
                   name: "default",
                   template: "{controller=Event}/{action=Monitor}/{eventId?}/{actionId?}"));

            app.UseMvc(routes => routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}"));

            if (AppConfig.LogToFile)
            Log.Logger = new LoggerConfiguration().WriteTo.File("Logs/logAssystHelper-.txt", rollingInterval: RollingInterval.Day).CreateLogger();

        }

        #region Блок инициализации

        //Загрузка настроек из файла appsettings.json
        private void InitAppConfig()
        {
            var login = Configuration.GetSection("AuthorizationSettings").GetSection("Login").Value;
            var password = Configuration.GetSection("AuthorizationSettings").GetSection("Password").Value;
            var cacheStorageTime = Convert.ToInt64(Configuration.GetSection("CacheStorageTime").Value);
            var longCacheStorageTime = Convert.ToInt64(Configuration.GetSection("LongCacheStorageTime").Value);
            var httpWaitResponceTime = Convert.ToInt64(Configuration.GetSection("HttpWaitResponceTime").Value);
            var httpAssystSynchronizationTime = Convert.ToInt64(Configuration.GetSection("AssystSynchronizationTime").Value);
            AppConfig.LoadConfig(login, password, cacheStorageTime, longCacheStorageTime, httpWaitResponceTime, httpAssystSynchronizationTime);
            AppConfig.ListUrl = Configuration.GetSection("UrlSettings").Get<List<UrlManager>>();
            AppConfig.HostUrl = Configuration["HostUrl"];
            AppConfig.MaxCountRecords = Convert.ToInt16(Configuration.GetSection("MaxCountRecords").Value);
            AppConfig.LogToFile = Convert.ToBoolean(Configuration["LogToFile"]);
        }

      #endregion
    }
}
