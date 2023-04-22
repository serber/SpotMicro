using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotMicro.App.Configuration;
using SpotMicro.App.Core;
using SpotMicro.App.Host;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json");
        builder.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<MotionConfiguration>(context.Configuration.GetSection("Motion"));

        services.AddSingleton<SpotMicroRobot>();
        services.AddHostedService<SpotMicroHost>();
    })
    .Build();

await host.RunAsync();