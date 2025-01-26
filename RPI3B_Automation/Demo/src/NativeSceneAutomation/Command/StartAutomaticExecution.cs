namespace NativeSceneAutomation.Command;

using MediatR;

public class StartAutomaticExecution : IRequest
{
    public StartAutomaticExecution(int sunriseDuration, 
                                   int sunsetDuration, 
                                   int dayDuration, 
                                   int nightDuration)
    {
        SunriseDuration = sunriseDuration;
        SunsetDuration  = sunsetDuration;
        DayDuration     = dayDuration;
        NightDuration   = nightDuration;
    }

    public int SunriseDuration { get; init; }
    public int SunsetDuration { get; init; }
    public int DayDuration { get; init; }
    public int NightDuration { get; init; }
}
