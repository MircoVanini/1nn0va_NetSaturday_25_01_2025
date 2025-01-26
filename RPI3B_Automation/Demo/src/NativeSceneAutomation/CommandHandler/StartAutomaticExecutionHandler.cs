namespace NativeSceneAutomation.CommandHandler;

using MediatR;
using NativeSceneAutomation.Command;
using NativeSceneAutomation.Workflows;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

public class StartAutomaticExecutionHandler : IRequestHandler<StartAutomaticExecution>
{
    private readonly IWorkflowHost _workflowHost;
    private readonly IWorkflowController _workflowController;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly WorkflowsExecutionState _state;

    public StartAutomaticExecutionHandler(IWorkflowHost workflowHost, IWorkflowController workflowController, IDistributedLockProvider lockProvider, WorkflowsExecutionState state)
    {
        _workflowHost       = workflowHost;
        _workflowController = workflowController;
        _lockProvider       = lockProvider;
        _state              = state;
    }

    public async Task Handle(StartAutomaticExecution request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_state.CurrentWorkflowId))
        {
            await _lockProvider.ReleaseLock(_state.CurrentWorkflowId);
            while (await _workflowController.TerminateWorkflow(_state.CurrentWorkflowId) != true)
                await Task.Delay(100);
            _state.CurrentWorkflowId = null;
        }

        var arg = new NativeSceneWorkflowInputModel
        {
            DayDuration     = request.DayDuration,
            NightDuration   = request.NightDuration,
            SunriseDuration = request.SunriseDuration,
            SunsetDuration  = request.SunsetDuration,
        };

        var id = await _workflowHost.StartWorkflow(nameof(NativeSceneWorkflow), arg);

        _state.CurrentWorkflowId = id;
    }
}
