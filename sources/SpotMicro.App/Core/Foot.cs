using System;
using System.Device.Pwm;
using Microsoft.Extensions.Logging;

namespace SpotMicro.App.Core;

internal sealed class Foot : IDisposable
{
    private readonly FootSide _footSide;

    private readonly int _shoulderReset;

    private readonly int _legReset;

    private readonly int _feetReset;

    private readonly ILogger _logger;

    private readonly SpotServoMotor _servoMotorShoulder;

    private readonly SpotServoMotor _servoMotorLeg;

    private readonly SpotServoMotor _servoMotorFeet;

    public Foot(FootSide footSide,
        PwmChannel pwmChannelShoulder,
        PwmChannel pwmChannelLeg,
        PwmChannel pwmChannelFeet,
        int shoulderReset,
        int legReset,
        int feetReset,
        ILogger logger)
    {
        _footSide = footSide;

        _shoulderReset = shoulderReset;
        _legReset = legReset;
        _feetReset = feetReset;
        _logger = logger;

        _servoMotorShoulder = new SpotServoMotor(pwmChannelShoulder, logger);
        _servoMotorLeg = new SpotServoMotor(pwmChannelLeg, logger);
        _servoMotorFeet = new SpotServoMotor(pwmChannelFeet, logger);
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing started");

        _servoMotorShoulder.Stop();
        _servoMotorLeg.Stop();
        _servoMotorFeet.Stop();

        _servoMotorShoulder?.Dispose();
        _servoMotorLeg?.Dispose();
        _servoMotorFeet?.Dispose();

        _logger.LogInformation("Disposing complete");
    }

    public void Reset()
    {
        _servoMotorShoulder.WriteAngle(_shoulderReset);
        _servoMotorLeg.WriteAngle(_legReset);
        _servoMotorFeet.WriteAngle(_feetReset);
    }

    public void StandUp()
    {
        if (_footSide == FootSide.Right)
        {
            _servoMotorShoulder.WriteAngle(90);
            _servoMotorLeg.WriteAngle(45);
            _servoMotorFeet.WriteAngle(120);
        }

        if (_footSide == FootSide.Left)
        {
            _servoMotorShoulder.WriteAngle(90);
            _servoMotorLeg.WriteAngle(135);
            _servoMotorFeet.WriteAngle(60);
        }
    }

    public void RaiseUp()
    {
        if (_footSide == FootSide.Right)
        {
            _servoMotorLeg.StepAngle(-10);
            _servoMotorFeet.StepAngle(30);
        }

        if (_footSide == FootSide.Left)
        {
            _servoMotorLeg.StepAngle(10);
            _servoMotorFeet.StepAngle(-30);
        }
    }

    public void RaiseDown()
    {
        if (_footSide == FootSide.Right)
        {
            _servoMotorLeg.StepAngle(10);
            _servoMotorFeet.StepAngle(-30);
        }

        if (_footSide == FootSide.Left)
        {
            _servoMotorLeg.StepAngle(-10);
            _servoMotorFeet.StepAngle(30);
        }
    }
}