namespace SpotMicro.App.Configuration;

internal sealed class Servo
{
    public string Name { get; set; }
    
    public int Channel { get; set; }
    
    public int RestAngle { get; set; }
}