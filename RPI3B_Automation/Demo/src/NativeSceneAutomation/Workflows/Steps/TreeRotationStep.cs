namespace NativeSceneAutomation.Workflows.Steps;

using NativeSceneAutomation.Models;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class TreeRotationStep : StepBodyAsync
{
    private readonly IHWService _hwService;

    public int Duration { get; set; }
    public bool Direction { get; set; }

    public TreeRotationStep(IHWService hwService)
    {
        _hwService = hwService;
    }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        try
        {
            if (!context.CancellationToken.IsCancellationRequested)
                await _hwService.StartRotateAsync(Duration, Direction);
        }
        catch (TaskCanceledException)
        {}
        
        return ExecutionResult.Next();
    }
}
