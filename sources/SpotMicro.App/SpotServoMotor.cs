using System;
using System.Device.Pwm;
using Iot.Device.ServoMotor;
using Microsoft.Extensions.Logging;

namespace SpotMicro.App;

public sealed class SpotServoMotor : IDisposable
{
    private const int AngleRange = 180;

    private const int MinimumPulseWidthMicroseconds = 500;

    private const int MaximumPulseWidthMicroseconds = 2500;

    private readonly ServoMotor _servoMotor;

    private readonly ILogger _logger;

    private double _angle = -1;

    public SpotServoMotor(PwmChannel pwmChannel,
        ILogger logger)
    {
        _logger = logger;
        _servoMotor = new ServoMotor(pwmChannel, AngleRange, MinimumPulseWidthMicroseconds, MaximumPulseWidthMicroseconds);
    }

    public void WriteAngle(double angle)
    {
        _logger.LogInformation("Foot {Current} -> {New}", _angle, angle);

        _angle = angle;
        _servoMotor.WriteAngle(_angle);
    }

    public void StepAngle(double angle)
    {
        _logger.LogInformation("Foot {Current} -> {New}", _angle, _angle + angle);

        _angle += angle;
        _servoMotor.WriteAngle(_angle);
    }

    public void Stop() => _servoMotor.Stop();

    public void Dispose()
    {
        _servoMotor?.Dispose();
    }
}