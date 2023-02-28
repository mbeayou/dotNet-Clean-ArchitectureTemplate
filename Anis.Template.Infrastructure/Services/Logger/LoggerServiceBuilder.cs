﻿using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using System;
using System.IO;

namespace Anis.Template.Infra.Services.Logger
{
    public class LoggerServiceBuilder
    {
        public static ILogger Build()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

            var serilogConfiguration = configuration.GetSection("Serilog");
            var seqUrl = serilogConfiguration["SeqUrl"];
            var appName = serilogConfiguration["AppName"];
            var appNamespace = serilogConfiguration["AppNamespace"];

            var logger = new LoggerConfiguration()
                            .Enrich.WithProperty("name", appName)
                            .Enrich.WithProperty("AppNamespace", appNamespace)
                            .ReadFrom.Configuration(configuration);

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                logger.WriteTo.Seq(
                    serverUrl: seqUrl
                );
                SelfLog.Enable(Console.Error);
            }

            return logger.CreateLogger();
        }
    }
}
