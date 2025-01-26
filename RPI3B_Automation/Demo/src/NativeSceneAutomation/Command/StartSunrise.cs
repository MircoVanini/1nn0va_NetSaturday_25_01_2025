namespace NativeSceneAutomation.Command;

using MediatR;

public class StartSunrise : IRequest
{
    public StartSunrise(int duration)
    {
        Duration = duration;
    }

    public int Duration { get; private set; }
}
