namespace NativeSceneAutomation.Workflows.Steps;

using NativeSceneAutomation.Models;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class SunriseStep : StepBodyAsync
{
    private readonly IHWService _hwService;

    public int Duration { get; set; }

    public SunriseStep(IHWService hwService)
    {
        _hwService = hwService;
    }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        try
        {
            if (!context.CancellationToken.IsCancellationRequested)
                await _hwService.DayPixelsColorsTransitionAsync(Duration);
        }
        catch (TaskCanceledException)
        {}
                
        return ExecutionResult.Next();
    }
}
