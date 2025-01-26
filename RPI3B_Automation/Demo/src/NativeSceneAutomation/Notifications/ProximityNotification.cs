namespace NativeSceneAutomation.Notifications;

using MediatR;

public class ProximityNotification : INotification
{
    public bool IsOnRange { get; set; }

    public ProximityNotification(bool isOnRange)
    {
        IsOnRange = isOnRange;
    }
}
