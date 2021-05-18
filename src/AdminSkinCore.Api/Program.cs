using AdminSkinCore.Api;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSKinCore.Common.Extension;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

string _namespace = typeof(Startup).Namespace;
string _appName = _namespace.Substring(_namespace.LastIndexOf('.', _namespace.LastIndexOf('.') - 1) + 1);

#region ���� IConfiguration ʵ����ȡ����
IConfiguration GetConfiguration()
    => (new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()).Build();
#endregion

#region ���� serilog
Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
    => new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", _appName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Http(configuration["Serilog:LogstashgUrl"] ?? "http://localhost:8080") // ELK + Serilog ʵ����־����
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
#endregion

var configuration = GetConfiguration();
Log.Logger = CreateSerilogLogger(configuration);
IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .CaptureStartupErrors(false)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();

var host = BuildWebHost(configuration, args);

#region ef core Ǩ��
host.MigrateDbContext<AdminSkinDbContext>((context, services) =>
{
    var env = services.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
    var logger = services.GetService(typeof(ILogger<AdminSkinDbContextSeed>)) as ILogger<AdminSkinDbContextSeed>;
    var bCryptService = services.GetService(typeof(IBCryptService)) as IBCryptService;

    new AdminSkinDbContextSeed(configuration, logger, bCryptService)
        .SeedDataMigrationAsync(context, env).Wait(); // ����Ҫ��wait() �����첽�Ļ���dbcontext����ᱻ��ǰ�ͷŵ�
});
#endregion

host.Run(); // ����Ӧ��
