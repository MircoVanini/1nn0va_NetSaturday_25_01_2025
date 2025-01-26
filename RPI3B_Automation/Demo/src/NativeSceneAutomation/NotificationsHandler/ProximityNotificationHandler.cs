namespace NativeSceneAutomation.NotificationsHandler;

using MediatR;
using NativeSceneAutomation.Models;
using NativeSceneAutomation.Notifications;
using NativeSceneAutomation.Workflows;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

public class ProximityNotificationHandler : INotificationHandler<ProximityNotification>
{
    private readonly IHWService _hwService;
    private readonly IWorkflowHost _workflowHost;
    private readonly WorkflowsExecutionState _state;

    public ProximityNotificationHandler(IHWService hwService, IWorkflowHost workflowHost, WorkflowsExecutionState state)
    {
        _hwService = hwService;
        _workflowHost = workflowHost;
        _state = state;
    }

    public async Task Handle(ProximityNotification notification, CancellationToken cancellationToken)
    {
        await _workflowHost.StartWorkflow(nameof(AnimationWorkflow), notification.IsOnRange);
    }
}
