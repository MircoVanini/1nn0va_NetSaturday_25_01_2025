namespace NativeSceneAutomation.Workflows;

using NativeSceneAutomation.Models;
using NativeSceneAutomation.Workflows.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class NativeSceneWorkflow : IWorkflow<NativeSceneWorkflowInputModel>
{
    private readonly ILogger<NativeSceneWorkflow> _logger;

    public string Id => nameof(NativeSceneWorkflow);

    public int Version => 1;

    public NativeSceneWorkflow(ILogger<NativeSceneWorkflow> logger)
    {
        _logger = logger;
    }

    public void Build(IWorkflowBuilder<NativeSceneWorkflowInputModel> builder)
    {
        builder
               .StartWith(context =>
               {
                   _logger.LogInformation("Starting NativeSceneWorkflow...");
                   return ExecutionResult.Next();
               })
               .While(_ => true).Do(x =>
               {
                   x.Then(context =>
                    {
                        _logger.LogInformation("Workflow NativeSceneWorkflow - day start.");
                        return ExecutionResult.Next();
                    })
                    .Then<SunriseStep>()
                        .Input(step => step.Duration, data => data.SunriseDuration)
                    .Then<WaitStep>()
                        .Input(step => step.Duration, data => data.DayDuration)
                    .Then<SunsetStep>()
                        .Input(step => step.Duration, data => data.SunsetDuration)
                    .Then<WaitStep>()
                        .Input(step => step.Duration, data => data.NightDuration)
                    .Then(context =>
                    {
                        _logger.LogInformation("Workflow NativeSceneWorkflow - day end.");
                        return ExecutionResult.Next();
                    })
                    .OnError(WorkflowErrorHandling.Suspend);
               });               
    }
}
