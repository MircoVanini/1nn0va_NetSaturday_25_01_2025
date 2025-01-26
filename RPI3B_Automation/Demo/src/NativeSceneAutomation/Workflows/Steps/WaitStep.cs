namespace NativeSceneAutomation.Workflows.Steps;

using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class WaitStep : StepBodyAsync
{
    public int Duration { get; set; }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        try
        {
            if (!context.CancellationToken.IsCancellationRequested)
                await Task.Delay(Duration, context.CancellationToken);
        }
        catch (TaskCanceledException)
        {}

        return ExecutionResult.Next();
    }
}
