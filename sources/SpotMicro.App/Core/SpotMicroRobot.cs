using Microsoft.Extensions.Options;
using SpotMicro.App.Configuration;
using System.Device.I2c;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Pwm;
using Microsoft.Extensions.Logging;

namespace SpotMicro.App.Core;

internal sealed class SpotMicroRobot : IDisposable
{
    private readonly IOptions<MotionConfiguration> _options;

    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger<SpotMicroRobot> _logger;

    private Foot _frontLeftFoot;

    private Foot _frontRightFoot;

    private Foot _rearLeftFoot;

    private Foot _rearRightFoot;

    private I2cDevice _device;

    private Pca9685 _pca9685;

    public SpotMicroRobot(IOptions<MotionConfiguration> options,
        ILoggerFactory loggerFactory,
        ILogger<SpotMicroRobot> logger)
    {
        _options = options;
        _loggerFactory = loggerFactory;
        _logger = logger;
    }

    public void Init()
    {
        _logger.LogInformation("Init started");
        const int busId = 1;
        var settings = new I2cConnectionSettings(busId, Pca9685.I2cAddressBase);

        _device = I2cDevice.Create(settings);
        _pca9685 = new Pca9685(_device);

        _logger.LogInformation("PCA9685 is ready on I2C bus {BusId} with address {DeviceAddress}",
            _device.ConnectionSettings.BusId,
            _device.ConnectionSettings.DeviceAddress);
        _logger.LogInformation("PWM Frequency: {PwmFrequency}Hz", _pca9685.PwmFrequency);

        InitFoot(FootLocation.Front, FootSide.Left, out _frontLeftFoot);
        InitFoot(FootLocation.Front, FootSide.Right, out _frontRightFoot);
        InitFoot(FootLocation.Rear, FootSide.Left, out _rearLeftFoot);
        InitFoot(FootLocation.Rear, FootSide.Right, out _rearRightFoot);

        _logger.LogInformation("Init complete");
    }

    public void Reset()
    {
        _frontLeftFoot.Reset();
        _frontRightFoot.Reset();
        _rearLeftFoot.Reset();
        _rearRightFoot.Reset();

        _logger.LogInformation("Reset complete");
    }

    public async Task StandUpAsync()
    {
        _rearLeftFoot.StandUp();
        _rearRightFoot.StandUp();

        await Task.Delay(300);

        _frontLeftFoot.StandUp();
        _frontRightFoot.StandUp();
        
        _logger.LogInformation("Stand up complete");
    }

    public async Task MarchAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _rearLeftFoot.RaiseUp();

            await Task.Delay(100, cancellationToken);

            _rearLeftFoot.RaiseDown();

            await Task.Delay(200, cancellationToken);

            _rearRightFoot.RaiseUp();

            await Task.Delay(100, cancellationToken);

            _rearRightFoot.RaiseDown();

            await Task.Delay(200, cancellationToken);

            _frontLeftFoot.RaiseUp();

            await Task.Delay(100, cancellationToken);

            _frontLeftFoot.RaiseDown();

            await Task.Delay(200, cancellationToken);

            _frontRightFoot.RaiseUp();

            await Task.Delay(100, cancellationToken);

            _frontRightFoot.RaiseDown();

            await Task.Delay(200, cancellationToken);
        }
    }


    public void Dispose()
    {
        _logger.LogInformation("Disposing started");

        _frontLeftFoot.Dispose();
        _frontRightFoot.Dispose();

        _rearLeftFoot.Dispose();
        _rearRightFoot.Dispose();

        _pca9685?.Dispose();

        _logger.LogInformation("Disposing complete");
    }

    private void InitFoot(FootLocation location,
        FootSide footSide,
        out Foot foot)
    {
        var shoulderServo = _options.Value.Servos.Single(x => x.Name.Equals($"{location}_shoulder_{footSide}", StringComparison.OrdinalIgnoreCase));
        var legServo = _options.Value.Servos.Single(x => x.Name.Equals($"{location}_leg_{footSide}", StringComparison.OrdinalIgnoreCase));
        var feetServo = _options.Value.Servos.Single(x => x.Name.Equals($"{location}_feet_{footSide}", StringComparison.OrdinalIgnoreCase));

        foot = new Foot(footSide,
            _pca9685.CreatePwmChannel(shoulderServo.Channel),
            _pca9685.CreatePwmChannel(legServo.Channel),
            _pca9685.CreatePwmChannel(feetServo.Channel),
            shoulderServo.RestAngle,
            legServo.RestAngle,
            feetServo.RestAngle,
            _loggerFactory.CreateLogger<Foot>());

        _logger.LogInformation("Init foot. Shoulder = {ShoulderChannel}, Leg = {LegChannel}, Feet = {FeetChannel}",
            shoulderServo.Channel,
            legServo.Channel,
            feetServo.Channel);
    }
}