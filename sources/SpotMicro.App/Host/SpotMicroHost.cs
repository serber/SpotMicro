using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SpotMicro.App.Core;

namespace SpotMicro.App.Host;

internal sealed class SpotMicroHost : IHostedService
{
    private readonly SpotMicroRobot _spotMicroRobot;

    public SpotMicroHost(SpotMicroRobot spotMicroRobot)
    {
        _spotMicroRobot = spotMicroRobot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _spotMicroRobot.Init();
        _spotMicroRobot.Reset();

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        await _spotMicroRobot.StandUpAsync();

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        await _spotMicroRobot.MarchAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _spotMicroRobot.Reset();

        return Task.CompletedTask;
    }
}