namespace NativeSceneAutomation.Command;

using MediatR;

public class StartSunset : IRequest
{
    public StartSunset(int duration)
    {
        Duration = duration;
    }

    public int Duration { get; private set; }
}
