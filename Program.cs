using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.Storage.SQLite;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

using ServManager.Worker;
using ServManager.Data;
using ServManager.SSHConnection;
using ServManager.Docker;
using ServManager.Utils;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://192.168.0.29:9200"))
    {
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
        IndexFormat = $"logs-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
    })
    .CreateLogger();

logger.Information("Web App is starting...");

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ServerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ServerContext") ?? throw new InvalidOperationException("Connection string 'ServManagerContext' not found.")));
}
else
{
    builder.Services.AddDbContext<ServerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ServerContext") ?? throw new InvalidOperationException("Connection string 'ProductionServManagerContext' not found")));
}

builder.Services.AddControllersWithViews();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage());

builder.Services.AddHangfireServer();

logger.Information("App Is Built...");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{       
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

logger.Information("HangFire server is Starting");
app.UseHangfireDashboard();

MyBackgroundTasks pinger = new();
RecurringJob.AddOrUpdate(
    "ping servers",
    () => pinger.PingServer(),
    Cron.Minutely);

logger.Information("Web app is running");

app.Run();

// IPAddress Address = IPAddress.Parse("192.168.0.29");

// bool status = SendPing.PingAddress(Address);
// string hostName = SendPing.GetHostName(Address);
// Console.WriteLine($"{Address} - {hostName} -> {status} - port: 1111 - {SendPing.PingHost(Address, 1111)}");

// DockerController docker = new();
// List<Container> containers = await docker.GetRunningContainers(Address);

// foreach (Container container in containers)
// {
//     Console.WriteLine($"{container.Name} is {container.State}");
// }


// string Username = "thomas";
// string Password = "Tom@szek18";

// SSH SshClient = new(Address, Username, Password);
// SshClient.Connect();
// string output_a = SshClient.ExecureCommand("docker ps --all");
// Console.WriteLine(output_a);
// SshClient.Disconnect();



