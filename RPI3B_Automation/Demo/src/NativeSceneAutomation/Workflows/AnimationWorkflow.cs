namespace NativeSceneAutomation.Workflows;

using NativeSceneAutomation.Workflows.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

public class AnimationWorkflow : IWorkflow<bool>
{
    private readonly ILogger<AnimationWorkflow> _logger;

    public string Id => nameof(AnimationWorkflow);

    public int Version => 1;

    public AnimationWorkflow(ILogger<AnimationWorkflow> logger)
    {
        _logger = logger;
    }

    public void Build(IWorkflowBuilder<bool> builder)
    {
        builder
             .StartWith(context =>
             {
                 _logger.LogInformation("Starting AnimationWorkflow...");
                 return ExecutionResult.Next();
             })
            .Then<TreeRotationStep>()
                .Input(step => step.Duration,  data => 5000)
                .Input(step => step.Direction, data => data)
            .Then(context =>
            {
                _logger.LogInformation("Workflow AnimationWorkflow complete.");
                return ExecutionResult.Next();
            })
            .OnError(WorkflowErrorHandling.Suspend);
    }
}
