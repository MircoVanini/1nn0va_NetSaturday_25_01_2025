namespace NativeSceneAutomation.CommandHandler;

using MediatR;
using NativeSceneAutomation.Command;
using NativeSceneAutomation.Models;
using NativeSceneAutomation.Workflows;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

public class StartSunriseHandler : IRequestHandler<StartSunrise>
{
    private readonly IHWService _hwService;
    private readonly IWorkflowHost _workflowHost;
    private readonly WorkflowsExecutionState _state;
    private readonly IWorkflowController _workflowController;
    private readonly IDistributedLockProvider _lockProvider;
  

    public StartSunriseHandler(IHWService hwService, IWorkflowHost workflowHost, IWorkflowController workflowController, IDistributedLockProvider lockProvider, WorkflowsExecutionState state)
    {
        _hwService          = hwService;
        _workflowHost       = workflowHost;
        _workflowController = workflowController;
        _lockProvider       = lockProvider;
        _state              = state;
    }

    public async Task Handle(StartSunrise request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_state.CurrentWorkflowId))
        {
            await _lockProvider.ReleaseLock(_state.CurrentWorkflowId);
            while (await _workflowController.TerminateWorkflow(_state.CurrentWorkflowId) != true)
                await Task.Delay(100);
            _state.CurrentWorkflowId = null;
        }

        await _hwService.DayPixelsColorsTransitionAsync(request.Duration);
    }
}
