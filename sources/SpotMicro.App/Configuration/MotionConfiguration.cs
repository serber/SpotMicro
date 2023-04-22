namespace SpotMicro.App.Configuration;

internal sealed class MotionConfiguration
{
    public int BoardAddress { get; set; }

    public Servo[] Servos { get; set; }
}