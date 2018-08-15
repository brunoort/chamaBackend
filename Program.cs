﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace Chama.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                 .UseKestrel()
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     var env = hostingContext.HostingEnvironment;
                     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();

                     Serilog.Log.Logger = new LoggerConfiguration().MinimumLevel.Error().WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "logs/{Date}.txt")).CreateLogger();
                 })
                .UseIISIntegration()
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddSerilog(dispose: true);
                     logging.AddConsole();
                     logging.AddDebug();
                 })
                 .UseNLog()
                 .UseStartup<Startup>()
                    .Build();


            webHost.Run();
        }
    }
}
